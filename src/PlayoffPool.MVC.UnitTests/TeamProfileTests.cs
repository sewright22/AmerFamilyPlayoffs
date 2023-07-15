using AmerFamilyPlayoffs.Data;
using AutoMapper;
using PlayoffPool.MVC;
using PlayoffPool.MVC.Mapping;
using PlayoffPool.MVC.Models;

namespace PlayoffPool.MVC.UnitTests
{
    [TestClass]
    public class TeamProfileTests
    {
        [TestMethod]
        public void AutoMapperConfigurationIsValid()
        {
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile<TeamProfile>());

            configuration.AssertConfigurationIsValid();
        }
    }
}