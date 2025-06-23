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
global using GKVK_Api.Interfaces;
//global using GKVK_Api.Mapper;
//global using GKVK_Api.Repositories;
global using GKVK_Api.Services;

//Microsoft libraries
global using Microsoft.EntityFrameworkCore;
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.OpenApi.Models;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Options;
global using Azure.Storage.Blobs;
global using Azure.Storage.Sas;


//Third party libraries
global using Serilog;
global using Serilog.Formatting.Compact;
global using SendGrid;
global using SendGrid.Helpers.Mail;
global using SendGrid.Helpers;
global using Riok.Mapperly; 
global using Riok.Mapperly.Abstractions;
global using Swashbuckle.AspNetCore.Filters;
global using Swashbuckle.AspNetCore.SwaggerGen;
global using Asp.Versioning;
global using Asp.Versioning.ApiExplorer;
