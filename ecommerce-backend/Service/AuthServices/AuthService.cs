﻿using Core.Entities.IdentitiyEntities;
using Core.Interfaces.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service.AuthServices
{
    public class AuthService(IConfiguration _configuration):IAuthService
    {
       // IConfiguration configuration = _configuration;
        public async Task<string> createtokenAsync(AppUser user, UserManager<AppUser> userManager)
        {

            // Private Claims (user defined - can change from user to other)
            var authClaims = new List<Claim>()
    {
        new Claim(ClaimTypes.GivenName, user.UserName),
        new Claim(ClaimTypes.Email, user.Email)
    };

            var userRoles = await userManager.GetRolesAsync(user);

            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            // secret key
            var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));

            // Token Object
            var token = new JwtSecurityToken(
                // Registered Claims
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JWT:DurationInMinutes"])),
                // Private Claims
                claims: authClaims,
                // Signature Key
                signingCredentials: new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256Signature)
            );

            // Create Token And Return It
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        }
    }
