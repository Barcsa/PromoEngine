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

        [Fact]
        public async Task ShouldFail_When_PromoCodeIsInvalid()
        {
            var promoRepo = new Mock<IPromoCodeRepository>();
            var submissionRepo = new Mock<ISubmissionRepository>();
            var timestampRepo = new Mock<IWinningTimestampRepository>();

            promoRepo.Setup(repo => repo.GetByCodeAsync("WRONG")).ReturnsAsync(value: null);

            var dto = new SubmissionRequestDto
            {
                FirstName = "barcsa",
                LastName = "gergo",
                Email = "barcsagergo@gmail.com",
                PhoneNumber = "+40746862228",
                PromoCode = "WRONG",
                AcceptedPrivacyPolicy = true,
                AcceptedGameRules = true
            };

            var service = new SubmissionService(promoRepo.Object, submissionRepo.Object, timestampRepo.Object);

            var result = await service.ProcessSubmissionAsync(dto);

            Assert.False(result.Success);
            Assert.Equal("A megadott promóciós kód érvénytelen.", result.Message);
        }

        [Fact]
        public async Task ShouldFail_When_PromoCodeAlreadyUsed()
        {
            var promoRepo = new Mock<IPromoCodeRepository>();
            var submissionRepo = new Mock<ISubmissionRepository>();
            var timestampRepo = new Mock<IWinningTimestampRepository>();

            var promo = new PromoCode { Id = 1, Code = "AAAAA", IsUsed = true };
            promoRepo.Setup(r => r.GetByCodeAsync("AAAAA")).ReturnsAsync(promo);

            var dto = new SubmissionRequestDto
            {
                FirstName = "barcsa",
                LastName = "gergo",
                Email = "barcsagergo@gmail.com",
                PhoneNumber = "+40746862228",
                PromoCode = "AAAAA",
                AcceptedPrivacyPolicy = true,
                AcceptedGameRules = true
            };

            var service = new SubmissionService(promoRepo.Object, submissionRepo.Object, timestampRepo.Object);

            var result = await service.ProcessSubmissionAsync(dto);

            Assert.False(result.Success);
            Assert.Equal("Ezt a promóciós kódot már felhasználták.", result.Message);
        }


        [Fact]
        public async Task ShouldFail_For_AllInvalidInputs()
        {
            var promoRepo = new Mock<IPromoCodeRepository>();
            var submissionRepo = new Mock<ISubmissionRepository>();
            var timestampRepo = new Mock<IWinningTimestampRepository>();
            var service = new SubmissionService(promoRepo.Object, submissionRepo.Object, timestampRepo.Object);

            var dto1 = new SubmissionRequestDto
            {
                FirstName = "",
                LastName = "gergo",
                Email = "barcsagergo@gmail.com",
                PhoneNumber = "+40746862228",
                PromoCode = "ABCDE",
                AcceptedPrivacyPolicy = true,
                AcceptedGameRules = true
            };
            var res1 = await service.ProcessSubmissionAsync(dto1);
            Assert.False(res1.Success);
            Assert.Equal("Kérlek, add meg a keresztnevedet.", res1.Message);

            var dto2 = new SubmissionRequestDto
            {
                FirstName = "barcsa",
                LastName = "",
                Email = "barcsagergo@gmail.com",
                PhoneNumber = "+40746862228",
                PromoCode = "ABCDE",
                AcceptedPrivacyPolicy = true,
                AcceptedGameRules = true
            };
            var res2 = await service.ProcessSubmissionAsync(dto2);
            Assert.False(res2.Success);
            Assert.Equal("Kérlek, add meg a vezetéknevedet.", res2.Message);

            var dto3 = new SubmissionRequestDto
            {
                FirstName = "barcsa",
                LastName = "gergo",
                Email = "",
                PhoneNumber = "+40746862228",
                PromoCode = "ABCDE",
                AcceptedPrivacyPolicy = true,
                AcceptedGameRules = true
            };
            var res3 = await service.ProcessSubmissionAsync(dto3);
            Assert.False(res3.Success);
            Assert.Equal("Add meg az e-mail címedet.", res3.Message);

            var dto4 = new SubmissionRequestDto
            {
                FirstName = "barcsa",
                LastName = "gergo",
                Email = "barcsagergo@gmail.com",
                PhoneNumber = "",
                PromoCode = "ABCDE",
                AcceptedPrivacyPolicy = true,
                AcceptedGameRules = true
            };
            var res4 = await service.ProcessSubmissionAsync(dto4);
            Assert.False(res4.Success);
            Assert.Equal("Add meg a telefonszámodat.", res4.Message);

            var dto5 = new SubmissionRequestDto
            {
                FirstName = "barcsa",
                LastName = "gergo",
                Email = "barcsagergo@gmail.com",
                PhoneNumber = "+40746862228",
                PromoCode = "",
                AcceptedPrivacyPolicy = true,
                AcceptedGameRules = true
            };
            var res5 = await service.ProcessSubmissionAsync(dto5);
            Assert.False(res5.Success);
            Assert.Equal("Add meg a promóciós kódot.", res5.Message);

            var dto6 = new SubmissionRequestDto
            {
                FirstName = "barcsa",
                LastName = "gergo",
                Email = "barcsagergo@gmail.com",
                PhoneNumber = "+40746862228",
                PromoCode = "ABCDE",
                AcceptedPrivacyPolicy = true,
                AcceptedGameRules = false
            };
            var res6 = await service.ProcessSubmissionAsync(dto6);
            Assert.False(res6.Success);
            Assert.Equal("A játékszabályzat elfogadása kötelező.", res6.Message);

            var dto7 = new SubmissionRequestDto
            {
                FirstName = "barcsa",
                LastName = "gergo",
                Email = "barcsagergo@gmail.com",
                PhoneNumber = "+40746862228",
                PromoCode = "ABCDE",
                AcceptedPrivacyPolicy = false,
                AcceptedGameRules = true
            };
            var res7 = await service.ProcessSubmissionAsync(dto7);
            Assert.False(res7.Success);
            Assert.Equal("Az adatvédelmi szabályzat elfogadása kötelező.", res7.Message);
        }

        [Fact]
        public async Task ShouldFail_When_UserAlreadyWonBefore()
        {
            var promoRepo = new Mock<IPromoCodeRepository>();
            var submissionRepo = new Mock<ISubmissionRepository>();
            var timestampRepo = new Mock<IWinningTimestampRepository>();

            var promo = new PromoCode { Id = 1, Code = "ABCDE", IsUsed = false };

            promoRepo.Setup(r => r.GetByCodeAsync("ABCDE")).ReturnsAsync(promo);
            submissionRepo.Setup(r => r.HasAlreadyWonAsync(It.IsAny<string>())).ReturnsAsync(true);
            timestampRepo.Setup(r => r.GetUnclaimedAsync()).ReturnsAsync(new List<WinningTimestamp>());

            var dto = new SubmissionRequestDto
            {
                FirstName = "barcsa",
                LastName = "gergo",
                Email = "barcsagergo@gmail.com",
                PhoneNumber = "+40746862228",
                PromoCode = "ABCDE",
                AcceptedPrivacyPolicy = true,
                AcceptedGameRules = true
            };

            var service = new SubmissionService(promoRepo.Object, submissionRepo.Object, timestampRepo.Object);
            var result = await service.ProcessSubmissionAsync(dto);

            Assert.True(result.Success);
            Assert.False(result.IsWinner);
            Assert.Null(result.PrizeType);
            Assert.Equal("Most nem volt szerencséd, Bajnok! A nagy győzelemhez sok küzdelem kell. Próbáld újra!", result.Message);
        }

        [Fact]
        public async Task ShouldReturnWeeklyWinner_When_WeeklyTimestampAvailable()
        {
            var promoRepo = new Mock<IPromoCodeRepository>();
            var submissionRepo = new Mock<ISubmissionRepository>();
            var timestampRepo = new Mock<IWinningTimestampRepository>();

            var promo = new PromoCode { Id = 1, Code = "WEEKY", IsUsed = false };

            promoRepo.Setup(r => r.GetByCodeAsync("WEEKY")).ReturnsAsync(promo);
            submissionRepo.Setup(r => r.HasAlreadyWonAsync(It.IsAny<string>())).ReturnsAsync(false);
            timestampRepo.Setup(r => r.GetUnclaimedAsync()).ReturnsAsync(
            new List<WinningTimestamp>
                {
                    new WinningTimestamp
                    {
                        Id = 99,
                        TargetTime = DateTime.UtcNow,
                        Type = "Weekly",
                        IsClaimed = false
                    }
                });

            var service = new SubmissionService(promoRepo.Object, submissionRepo.Object, timestampRepo.Object);

            var dto = new SubmissionRequestDto
            {
                FirstName = "Barcsa",
                LastName = "Gergo",
                Email = "barcsagergo@gmail.com",
                PhoneNumber = "+40746862228",
                PromoCode = "WEEKY",
                AcceptedPrivacyPolicy = true,
                AcceptedGameRules = true
            };

            var result = await service.ProcessSubmissionAsync(dto);

            Assert.True(result.Success);
            Assert.True(result.IsWinner);
            Assert.Equal("Weekly", result.PrizeType);
            Assert.Equal("Nyertél, Bajnok! Megnyerted a heti nyereményt!", result.Message);
        }

    }
}
