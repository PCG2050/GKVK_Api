global using System;
global using System.Text;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;
global using System.ComponentModel.DataAnnotations;
global using System.ComponentModel.DataAnnotations.Schema;
global using System.Security.Cryptography;
global using System.IdentityModel.Tokens.Jwt;
global using System.Security.Claims;
global using System.Text.Json.Serialization;

//FileSpace
global using GKVK_Api;
global using GKVK_Api.Controllers;
global using GKVK_Api.Models.Domain;
global using GKVK_Api.Models.DTO;

//MIcrosoft libraries
global using Microsoft.EntityFrameworkCore;


//Third party libraries
global using Serilog;
global using SendGrid.Helpers;