using System;
using MTD.CouchBot.Dals;
using MTD.CouchBot.Dals.Implementations;
using Xunit;

namespace MTD.CouchBot.Tests.Dals
{
    public class GroupDalTests
    {
        private readonly IGroupDal _groupDal;

        public GroupDalTests()
        {
            //_groupDal = new GroupDal(new IConfiguration);
        }

        [Fact]
        public void CanGetAllGuildGroups()
        {

        }
    }
}
