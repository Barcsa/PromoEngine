using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PromoEngine.Services;
using PromoEngine.Repositories.Interfaces;
using PromoEngine.DTOs;
using PromoEngine.Models;

namespace PromoEngine.Tests
{
    public class SubmissionServiceTests
    {
        [Fact]
        public async Task ShouldReturnWinner_When_ValidSubmission()
        {
            var promoRepo = new Mock<IPromoCodeRepository>();
            var submissionRepo = new Mock<ISubmissionRepository>();
            var timestampRepo = new Mock<IWinningTimestampRepository>();

            var promo = new PromoCode { Id = 1, Code = "ABCDE", IsUsed = false };

            promoRepo.Setup(r => r.GetByCodeAsync("ABCDE")).ReturnsAsync(promo);
            submissionRepo.Setup(r => r.HasAlreadyWonAsync(It.IsAny<string>())).ReturnsAsync(false);
            timestampRepo.Setup(r => r.GetUnclaimedAsync()).ReturnsAsync(
                new List<WinningTimestamp>
                {
                    new WinningTimestamp
                    {
                        Id = 1,
                        TargetTime = DateTime.UtcNow,
                        Type = "Daily",
                        IsClaimed = false
                    }
                });

            var service = new SubmissionService(promoRepo.Object, submissionRepo.Object, timestampRepo.Object);

            var dto = new SubmissionRequestDto();
            dto.FirstName = "Barcsa";
            dto.LastName = "Gergo";
            dto.Email = "barcsagergo@gmail.com";
            dto.PhoneNumber = "+40746862228";
            dto.PromoCode = "ABCDE";
            dto.AcceptedPrivacyPolicy = true;
            dto.AcceptedGameRules = true;

            var result = await service.ProcessSubmissionAsync(dto);

            Assert.True(result.Success);
            Assert.True(result.IsWinner);
            Assert.Equal("Daily", result.PrizeType);
        }
    }
}
