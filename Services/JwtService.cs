public class JwtService
{
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _dbContext;

    public JwtService(IConfiguration configuration, ApplicationDbContext dbContext)
    {
        _configuration = configuration;
        _dbContext = dbContext;
    }

    /// <summary>
    /// Generate JWT Access Token
    /// </summary>
    public string GenerateJwtToken(string userId, string email, string role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JwtSettings:TokenExpirationInMinutes"]!)),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Generate and Store Refresh Token
    /// </summary>
    public async Task<string> GenerateAndStoreRefreshToken(string userId)
    {
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        var newRefreshToken = new RefreshToken
        {
            FK_UserId = int.Parse(userId),
            Token = refreshToken,
            ExpiryDate = DateTime.UtcNow.AddDays(double.Parse(_configuration["JwtSettings:RefreshTokenExpirationInDays"]!)) // Refresh token expiry (e.g., 7 days)
        };

        _dbContext.RefreshTokens.Add(newRefreshToken);
        await _dbContext.SaveChangesAsync();

        return refreshToken;
    }

    /// <summary>
    /// Validate Refresh Token and Generate New Access Token
    /// </summary>
    public async Task<(string AccessToken, string RefreshToken)?> RefreshAccessToken(string refreshToken)
    {
        var storedToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiryDate < DateTime.UtcNow)
        {
            return null; // Invalid refresh token
        }

        var user = await _dbContext.Users.FindAsync(storedToken.FK_UserId);
        if (user == null) return null;

        // Generate new tokens
        var newAccessToken = GenerateJwtToken(user.UserId.ToString(), user.Email, user.Role.ToString());
        var newRefreshToken = await GenerateAndStoreRefreshToken(user.UserId.ToString());

        // Optionally revoke the old refresh token
        storedToken.IsRevoked = true;
        await _dbContext.SaveChangesAsync();

        return (newAccessToken, newRefreshToken);
    }
}
