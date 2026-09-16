using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LMS.API.Core.Types;
using LMS.API.Core.Workflows;
using LMS.API.DTOs.Auth;
using LMS.API.Models;
using LMS.API.Shell;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace LMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(
        IConfiguration configuration,
        UserManager<ApplicationUser> userManager) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<ActionResult<ResultModel>> Login([FromBody] LoginModel model)
        {
            var user = await userManager.FindByNameAsync(model.Username);
            var passwordOk = user is not null &&
                             await userManager.CheckPasswordAsync(user, model.Password);
            var roles = user is not null
                ? await userManager.GetRolesAsync(user)
                : Array.Empty<string>();

            var loginInput = new LoginInput(
                FoundUserId: user?.Id,
                Username: model.Username,
                PasswordMatches: passwordOk,
                Roles: (IReadOnlyList<string>)roles,
                Profile: user is null ? null : new UserProfile(
                    user.Id, user.CreatedAt, user.UpdatedAt,
                    user.Email ?? "", user.FirstName ?? "", user.LastName ?? "",
                    user.ImageUrl, user.CourseId));

            var result = LoginWorkflow.Execute(loginInput);

            if (result is not WorkflowResult<LoginPayload>.Ok ok)
                return this.ToActionResult(result);

            var jwt = configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwt["SecretKey"]!));

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, ok.Value.Profile.Id),
                new(ClaimTypes.Name, model.Username),
                new(ClaimTypes.Role, ok.Value.Role)
            };

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60 * 48),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            var resultModel = ok.Value.ToResultModel();
            resultModel.Token = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(resultModel);
        }
    }
}