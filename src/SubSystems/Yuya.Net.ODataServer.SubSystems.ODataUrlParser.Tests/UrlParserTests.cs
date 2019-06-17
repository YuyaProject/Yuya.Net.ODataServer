using Moq;
using Shouldly;
using System;
using Xunit;
using Yuya.Net.ODataServer.SubSystems.ODataUrlParser.QueryDetail;

namespace Yuya.Net.ODataServer.SubSystems.ODataUrlParser.Tests
{
	public class UrlParserTests : IDisposable
	{
		private MockRepository mockRepository;

		public UrlParserTests()
		{
			this.mockRepository = new MockRepository(MockBehavior.Strict);
		}

		public void Dispose()
		{
			this.mockRepository.VerifyAll();
		}

		private UrlParser CreateUrlParser()
		{
			return new UrlParser();
		}

		private ParseOptions CreateParseOptions()
		{
			return new ParseOptions()
			{
				BaseUrl = "http://www.demo.com/aaa/",
				ODataDirectory = "odata"
			};
		}

		[Fact]
		public void ParseQuery_NullParameter_ThrowException()
		{
			// Arrange
			var unitUnderTest = this.CreateUrlParser();
			var options = new ParseOptions();
			string urlString = null;

			// Act
			Assert.Throws<ArgumentNullException>(() => unitUnderTest.ParseQuery(urlString, options));

			// Assert
			//Assert.True(false);
		}

		[Fact]
		public void ParseQuery_EmptyStringParameter_ThrowException()
		{
			// Arrange
			var unitUnderTest = this.CreateUrlParser();
			var options = new ParseOptions();
			string urlString = "";

			// Act
			Assert.Throws<ArgumentException>(() => unitUnderTest.ParseQuery(urlString, options));

			// Assert
			//Assert.True(false);
		}

		[Fact]
		public void ParseQuery_WrongFormatStringParameter_ThrowException()
		{
			// Arrange
			var unitUnderTest = this.CreateUrlParser();
			var options = CreateParseOptions();
			string urlString = "abc";

			// Act
			var exception = Assert.Throws<ArgumentException>(() => unitUnderTest.ParseQuery(urlString, options));

			// Assert
			//Assert.True(false);
			exception.ParamName.ShouldBe("urlString");
			//exception.InnerException.ShouldNotBeNull();
			//exception.InnerException.ShouldBeOfType<UriFormatException>();
		}

		[Fact]
		public void ParseQuery_WrongFormatStringParameter2_ThrowException()
		{
			// Arrange
			var unitUnderTest = this.CreateUrlParser();
			var options = CreateParseOptions();
			string urlString = "http://www.demo.com/aaa/odata/product(";

			// Act
			var exception = Assert.Throws<ArgumentException>(() => unitUnderTest.ParseQuery(urlString, options));

			// Assert
			//Assert.True(false);
			exception.ParamName.ShouldBe("urlString");
			//exception.InnerException.ShouldNotBeNull();
			//exception.InnerException.ShouldBeOfType<UriFormatException>();
		}

		[Fact]
		public void ParseQuery_OneEntityListStringParameter_QueryDetailInstance()
		{
			// Arrange
			var unitUnderTest = this.CreateUrlParser();
			var options = CreateParseOptions();
			string urlString = "http://www.demo.com/aaa/odata/product";

			// Act
			var result = unitUnderTest.ParseQuery(urlString, options);

			// Assert
			result.ShouldNotBeNull();
			result.ParseOptions.ShouldBe(options);
			result.EntitySelectors.ShouldNotBeNull();
			result.EntitySelectors.Count.ShouldBe(1);
			result.EntitySelectors[0].ShouldBeOfType<ListEntitySelector>();
			result.EntitySelectors[0].EntityName.ShouldBe("product");
			result.ColumnSelectors.ShouldNotBeNull();
			result.ColumnSelectors.Count.ShouldBe(0);
			result.Top.ShouldBeNull();
			result.Skip.ShouldBeNull();
		}

		[Fact]
		public void ParseQuery_OneEntitySingleStringParameter_QueryDetailInstance()
		{
			// Arrange
			var unitUnderTest = this.CreateUrlParser();
			var options = CreateParseOptions();
			string urlString = "http://www.demo.com/aaa/odata/product(1)";

			// Act
			var result = unitUnderTest.ParseQuery(urlString, options);

			// Assert
			result.ShouldNotBeNull();
			result.ParseOptions.ShouldBe(options);
			result.EntitySelectors.ShouldNotBeNull();
			result.EntitySelectors.Count.ShouldBe(1);
			result.EntitySelectors[0].ShouldBeOfType<SingleEntitySelector>();
			result.EntitySelectors[0].EntityName.ShouldBe("product");
			((SingleEntitySelector)(result.EntitySelectors[0])).Key.ShouldBe("1");
			result.ColumnSelectors.ShouldNotBeNull();
			result.ColumnSelectors.Count.ShouldBe(0);
			result.Top.ShouldBeNull();
			result.Skip.ShouldBeNull();
		}

		[Fact]
		public void ParseQuery_OneEntityListStringParameterWithColumns_QueryDetailInstance()
		{
			// Arrange
			var unitUnderTest = this.CreateUrlParser();
			var options = CreateParseOptions();
			string urlString = "http://www.demo.com/aaa/odata/product?$select=id,name";

			// Act
			var result = unitUnderTest.ParseQuery(urlString, options);

			// Assert
			result.ShouldNotBeNull();
			result.ParseOptions.ShouldBe(options);
			result.EntitySelectors.ShouldNotBeNull();
			result.EntitySelectors.Count.ShouldBe(1);
			result.EntitySelectors[0].ShouldBeOfType<ListEntitySelector>();
			result.EntitySelectors[0].EntityName.ShouldBe("product");
			result.ColumnSelectors.ShouldNotBeNull();
			result.ColumnSelectors.Count.ShouldBe(2);
			result.ColumnSelectors[0].ShouldBe("id");
			result.ColumnSelectors[1].ShouldBe("name");
			result.Top.ShouldBeNull();
			result.Skip.ShouldBeNull();
		}

		[Fact]
		public void ParseQuery_OneEntityListStringParameterWithColumnsAndTop_QueryDetailInstance()
		{
			// Arrange
			var unitUnderTest = this.CreateUrlParser();
			var options = CreateParseOptions();
			string urlString = "http://www.demo.com/aaa/odata/product?$select=id,name&$top=5";

			// Act
			var result = unitUnderTest.ParseQuery(urlString, options);

			// Assert
			result.ShouldNotBeNull();
			result.ParseOptions.ShouldBe(options);
			result.EntitySelectors.ShouldNotBeNull();
			result.EntitySelectors.Count.ShouldBe(1);
			result.EntitySelectors[0].ShouldBeOfType<ListEntitySelector>();
			result.EntitySelectors[0].EntityName.ShouldBe("product");
			result.ColumnSelectors.ShouldNotBeNull();
			result.ColumnSelectors.Count.ShouldBe(2);
			result.ColumnSelectors[0].ShouldBe("id");
			result.ColumnSelectors[1].ShouldBe("name");
			result.Top.ShouldBe(5);
			result.Skip.ShouldBeNull();
		}

		[Fact]
		public void ParseQuery_OneEntityListStringParameterWithColumnsAndTopAndSkip_QueryDetailInstance()
		{
			// Arrange
			var unitUnderTest = this.CreateUrlParser();
			var options = CreateParseOptions();
			string urlString = "http://www.demo.com/aaa/odata/product?$select=id,name&$top=5&$skip=5";

			// Act
			var result = unitUnderTest.ParseQuery(urlString, options);

			// Assert
			result.ShouldNotBeNull();
			result.ParseOptions.ShouldBe(options);
			result.EntitySelectors.ShouldNotBeNull();
			result.EntitySelectors.Count.ShouldBe(1);
			result.EntitySelectors[0].ShouldBeOfType<ListEntitySelector>();
			result.EntitySelectors[0].EntityName.ShouldBe("product");
			result.ColumnSelectors.ShouldNotBeNull();
			result.ColumnSelectors.Count.ShouldBe(2);
			result.ColumnSelectors[0].ShouldBe("id");
			result.ColumnSelectors[1].ShouldBe("name");
			result.Top.ShouldBe(5);
			result.Skip.ShouldBe(5);
		}

		[Fact]
		public void ParseQuery_OneEntityListStringParameterWithTopAndSkip_QueryDetailInstance()
		{
			// Arrange
			var unitUnderTest = this.CreateUrlParser();
			var options = CreateParseOptions();
			string urlString = "http://www.demo.com/aaa/odata/product?$top=5&$skip=5";

			// Act
			var result = unitUnderTest.ParseQuery(urlString, options);

			// Assert
			result.ShouldNotBeNull();
			result.ParseOptions.ShouldBe(options);
			result.EntitySelectors.ShouldNotBeNull();
			result.EntitySelectors.Count.ShouldBe(1);
			result.EntitySelectors[0].ShouldBeOfType<ListEntitySelector>();
			result.EntitySelectors[0].EntityName.ShouldBe("product");
			result.ColumnSelectors.ShouldNotBeNull();
			result.ColumnSelectors.Count.ShouldBe(0);
			result.Top.ShouldBe(5);
			result.Skip.ShouldBe(5);
		}


		[Fact]
		public void ParseQuery_OneEntityListStringParameterWithWrongFormatParameter_ThrowException()
		{
			// Arrange
			var unitUnderTest = this.CreateUrlParser();
			var options = CreateParseOptions();
			string urlString = "http://www.demo.com/aaa/odata/product?$to=5a";

			// Act
			var exception = Assert.Throws<ArgumentException>(() => unitUnderTest.ParseQuery(urlString, options));

			// Assert
			exception.ParamName.ShouldBe("urlString");
		}

		[Fact]
		public void ParseQuery_OneEntityListStringParameterWithWrongFormatParameter2_ThrowException()
		{
			// Arrange
			var unitUnderTest = this.CreateUrlParser();
			var options = CreateParseOptions();
			string urlString = "http://www.demo.com/aaa/odata/product?$to=";

			// Act
			var exception = Assert.Throws<ArgumentException>(() => unitUnderTest.ParseQuery(urlString, options));

			// Assert
			exception.ParamName.ShouldBe("urlString");
		}


		[Fact]
		public void ParseQuery_OneEntityListStringParameterWithWrongFormatParameter3_ThrowException()
		{
			// Arrange
			var unitUnderTest = this.CreateUrlParser();
			var options = CreateParseOptions();
			string urlString = "http://www.demo.com/aaa/odata/product?$to";

			// Act
			var exception = Assert.Throws<ArgumentException>(() => unitUnderTest.ParseQuery(urlString, options));

			// Assert
			exception.ParamName.ShouldBe("urlString");
		}


		[Fact]
		public void ParseQuery_OneEntityListStringParameterWithTopWrongFormat_ThrowException()
		{
			// Arrange
			var unitUnderTest = this.CreateUrlParser();
			var options = CreateParseOptions();
			string urlString = "http://www.demo.com/aaa/odata/product?$top=5a";

			// Act
			var exception = Assert.Throws<ArgumentException>(() => unitUnderTest.ParseQuery(urlString, options));

			// Assert
			exception.ParamName.ShouldBe("urlString");
		}




		[Fact]
		public void ParseQuery_OneEntityListStringParameterWithSkipWrongFormat_ThrowException()
		{
			// Arrange
			var unitUnderTest = this.CreateUrlParser();
			var options = CreateParseOptions();
			string urlString = "http://www.demo.com/aaa/odata/product?$skip=5a";

			// Act
			var exception = Assert.Throws<ArgumentException>(() => unitUnderTest.ParseQuery(urlString, options));

			// Assert
			exception.ParamName.ShouldBe("urlString");
		}
	}
}