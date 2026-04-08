using CleanMonolith.Domain.Entities;
using CleanMonolith.Infrastructure.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanMonolith.Infrastructure.Mappers
{
    public static class RefreshTokenMapper
    {
        public static RefreshToken ToDomain(Tbl_RefreshToken entity)
        {
            return new RefreshToken
            {
                Id = entity.Id,
                UserId = (int)entity.UserId,
                TokenHash = entity.TokenHash,
                JwtId = entity.JwtId,
                CreatedAt = entity.CreatedAt,
                ExpiryDate = entity.ExpiryDate,
                IsUsed = entity.IsUsed,
                IsRevoked = entity.IsRevoked
            };
        }

        public static Tbl_RefreshToken ToEntity(RefreshToken domain)
        {
            return new Tbl_RefreshToken
            {
                Id = domain.Id,
                UserId = domain.UserId,
                TokenHash = domain.TokenHash,
                JwtId = domain.JwtId,
                CreatedAt = domain.CreatedAt,
                ExpiryDate = domain.ExpiryDate,
                IsUsed = domain.IsUsed,
                IsRevoked = domain.IsRevoked
            };
        }
    }
}
