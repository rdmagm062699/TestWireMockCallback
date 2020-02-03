namespace AcceptanceTests.StepDefinitions
{
    using FluentAssertions;
    using TechTalk.SpecFlow;
    using TechTalk.SpecFlow.Assist;
    using Xunit;

    [Binding]
    public class Something
    {
        [Given(@"This thing happens")]
        public void ThisThingHappens()
        {
        }

        [Then(@"This is the result")]
        public void ThisIsTheResult()
        {
            true.Should().Be(false);
        }
    }
}