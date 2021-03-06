// -- FILE ------------------------------------------------------------------
// name       : RtfInterpreterTest.cs
// project    : RTF Framelet
// created    : Leon Poyyayil - 2008.05.20
// language   : c#
// environment: .NET 2.0
// copyright  : (c) 2004-2012 by Itenso GmbH, Switzerland
// --------------------------------------------------------------------------
using System;
using System.Globalization;
using System.IO;
using System.Text;
using NUnit.Framework;
using Itenso.Sys.Test;
using Itenso.Rtf.Interpreter;
using Itenso.Rtf.Model;
using Itenso.Rtf.Parser;
using Itenso.Rtf.Support;
using Itenso.Rtf.Converter.Text;

namespace Itenso.Rtf.InterpreterTests
{

	// ------------------------------------------------------------------------
// ReSharper disable ExpressionIsAlwaysNull
	[TestFixture]
	public sealed class RtfInterpreterTest : TestUnitBase
	{

		// ----------------------------------------------------------------------
		[Test]
		public void ListSupportTest()
		{
			const RtfParserListenerLogger parserLogger = null;
			//parserLogger = new RtfParserListenerLogger();

			//string testCaseName = "RtfInterpreterTest_9";
			const string testCaseName = "RtfInterpreterTest_10";
			IRtfGroup rtfStructure = RtfParserTool.Parse( GetTestResource( testCaseName + ".rtf" ), parserLogger );

			const RtfInterpreterListenerLogger interpreterLogger = null;
			//interpreterLogger = new RtfInterpreterListenerLogger();

			RtfTextConverter textConverter = new RtfTextConverter();

			RtfInterpreterTool.Interpret( rtfStructure, textConverter, interpreterLogger );

			string plainText = textConverter.PlainText;
			AssertEqualLines( "list support:",
				new StreamReader( GetTestResource( testCaseName + ".txt" ), RtfSpec.AnsiEncoding ),
				new StringReader( plainText ) );
		} // ListSupportTest

		// ----------------------------------------------------------------------
		[Test]
		public void ImageFormatDecodingTest()
		{
			const RtfParserListenerLogger parserLogger = null;
			//parserLogger = new RtfParserListenerLogger();

			const RtfInterpreterListenerLogger interpreterLogger = null;
			//interpreterLogger = new RtfInterpreterListenerLogger();

// ReSharper disable RedundantExplicitArrayCreation
			int[] imageResources = new int[] { 4, 5, 6, 7, 8, 19 };
// ReSharper restore RedundantExplicitArrayCreation
			for ( int i = 0; i < imageResources.Length; i++ )
			{
				string testCaseName = BuildTestResourceName( "", imageResources[ i ], false, "rtf" );
				IRtfGroup rtfStructure =
					RtfParserTool.Parse( GetTestResource( testCaseName ), parserLogger );
				IRtfDocument rtfDoc = RtfInterpreterTool.BuildDoc( rtfStructure, interpreterLogger );
				Assert.IsNotNull( rtfDoc );
				bool imageFound = false;
				foreach ( IRtfVisual visual in rtfDoc.VisualContent )
				{
					if ( visual.Kind == RtfVisualKind.Image )
					{
						IRtfVisualImage img = (IRtfVisualImage)visual;
						Assert.IsNotNull( img.ImageForDrawing );
						imageFound = true;
						//Console.WriteLine( "image: " + img.ImageForDrawing );
					}
				}
				Assert.IsTrue( imageFound, "no image found in test case " + testCaseName );
			}
		} // ImageFormatDecodingTest

		// ----------------------------------------------------------------------
		[Test]
		public void ImageTest()
		{
			const RtfParserListenerLogger parserLogger = null;
			//parserLogger = new RtfParserListenerLogger();
			IRtfGroup rtfStructure =
				RtfParserTool.Parse( GetTestResource( "RtfInterpreterTest_4.rtf" ), parserLogger );
			Assert.IsNotNull( rtfStructure );

			const RtfInterpreterListenerLogger interpreterLogger = null;
			//interpreterLogger = new RtfInterpreterListenerLogger();

			IRtfDocument rtfDoc = RtfInterpreterTool.BuildDoc( rtfStructure, interpreterLogger );
			Assert.IsNotNull( rtfDoc );
			IRtfVisualCollection rtfVisuals = rtfDoc.VisualContent;

			Assert.AreEqual( RtfVisualKind.Image, rtfVisuals[ 4 ].Kind );
			IRtfVisualImage img = (IRtfVisualImage)rtfVisuals[ 4 ];
			Assert.AreEqual( RtfVisualImageFormat.Jpg, img.Format );
			Assert.AreEqual( 100, img.Width );
			Assert.AreEqual( 142, img.Height );
			Assert.AreEqual( 720, img.DesiredWidth );
			Assert.AreEqual( 1020, img.DesiredHeight );
		} // ImageTest

		// ----------------------------------------------------------------------
		[Test]
		public void TextAlignmentTest()
		{
			const RtfParserListenerLogger parserLogger = null;
			//parserLogger = new RtfParserListenerLogger();
			IRtfGroup rtfStructure =
				RtfParserTool.Parse( GetTestResource( "RtfInterpreterTest_3.rtf" ), parserLogger );
			Assert.IsNotNull( rtfStructure );

			const RtfInterpreterListenerLogger interpreterLogger = null;
			//interpreterLogger = new RtfInterpreterListenerLogger();

			IRtfDocument rtfDoc = RtfInterpreterTool.BuildDoc( rtfStructure, interpreterLogger );
			Assert.IsNotNull( rtfDoc );
			IRtfVisualCollection rtfVisuals = rtfDoc.VisualContent;

			Assert.AreEqual( 8, rtfVisuals.Count );
			Assert.AreEqual( RtfVisualKind.Text, rtfVisuals[ 0 ].Kind );
			Assert.AreEqual( "left aligned", ((IRtfVisualText)rtfVisuals[ 0 ]).Text );
			Assert.AreEqual( RtfTextAlignment.Left, ((IRtfVisualText)rtfVisuals[ 0 ]).Format.Alignment );
			Assert.AreEqual( RtfVisualKind.Text, rtfVisuals[ 2 ].Kind );
			Assert.AreEqual( "centered", ((IRtfVisualText)rtfVisuals[ 2 ]).Text );
			Assert.AreEqual( RtfTextAlignment.Center, ((IRtfVisualText)rtfVisuals[ 2 ]).Format.Alignment );
			Assert.AreEqual( RtfVisualKind.Text, rtfVisuals[ 4 ].Kind );
			Assert.AreEqual( "right aligned", ((IRtfVisualText)rtfVisuals[ 4 ]).Text );
			Assert.AreEqual( RtfTextAlignment.Right, ((IRtfVisualText)rtfVisuals[ 4 ]).Format.Alignment );
			Assert.AreEqual( RtfVisualKind.Text, rtfVisuals[ 6 ].Kind );
			Assert.AreEqual( "block aligned", ((IRtfVisualText)rtfVisuals[ 6 ]).Text );
			Assert.AreEqual( RtfTextAlignment.Justify, ((IRtfVisualText)rtfVisuals[ 6 ]).Format.Alignment );
		} // TextAlignmentTest

		// ----------------------------------------------------------------------
		[Test]
		public void TextAlignmentTestFixed()
		{
			const RtfParserListenerLogger parserLogger = null;
			//parserLogger = new RtfParserListenerLogger();
			IRtfGroup rtfStructure =
				RtfParserTool.Parse( GetTestResource( "RtfInterpreterTest_23.rtf" ), parserLogger );
			Assert.IsNotNull( rtfStructure );

			const RtfInterpreterListenerLogger interpreterLogger = null;
			//interpreterLogger = new RtfInterpreterListenerLogger();

			IRtfDocument rtfDoc = RtfInterpreterTool.BuildDoc( rtfStructure, interpreterLogger );
			Assert.IsNotNull( rtfDoc );
			IRtfVisualCollection rtfVisuals = rtfDoc.VisualContent;

			Assert.AreEqual( 2, rtfVisuals.Count );
			Assert.AreEqual( RtfVisualKind.Text, rtfVisuals[ 0 ].Kind );
			Assert.AreEqual( "Simple text", ( (IRtfVisualText)rtfVisuals[ 0 ] ).Text );
			Assert.AreEqual( RtfTextAlignment.Right, ( (IRtfVisualText)rtfVisuals[ 0 ] ).Format.Alignment );
		} // TextAlignmentTestFixed

		// ----------------------------------------------------------------------
		[Test]
		public void DocumentBuilderTest()
		{
			const RtfParserListenerLogger parserLogger = null;
			//parserLogger = new RtfParserListenerLogger();
			IRtfGroup rtfStructure =
				RtfParserTool.Parse( GetTestResource( "RtfInterpreterTest_1.rtf" ), parserLogger );
			Assert.IsNotNull( rtfStructure );

			const RtfInterpreterListenerLogger interpreterLogger = null;
			//interpreterLogger = new RtfInterpreterListenerLogger();

			RtfInterpreterListenerDocumentBuilder docBuilder =
				new RtfInterpreterListenerDocumentBuilder();

			RtfInterpreterTool.Interpret( rtfStructure, docBuilder, interpreterLogger );
			IRtfDocument doc = docBuilder.Document;
			Assert.IsNotNull( doc );

			Assert.AreEqual( "TX_RTF32 14.0.520.501", doc.Generator );

			Assert.AreEqual( 3, doc.FontTable.Count );
			Assert.AreEqual( RtfFontKind.Swiss, doc.FontTable[ 0 ].Kind );
			Assert.AreEqual( RtfFontPitch.Variable, doc.FontTable[ 0 ].Pitch );
			Assert.AreEqual( 0, doc.FontTable[ 0 ].CharSet );
			Assert.AreEqual( 1252, doc.FontTable[ 0 ].CodePage );
			Assert.AreEqual( "Arial", doc.FontTable[ 0 ].Name );

			Assert.AreEqual( RtfFontKind.Swiss, doc.FontTable[ 1 ].Kind );
			Assert.AreEqual( RtfFontPitch.Variable, doc.FontTable[ 1 ].Pitch );
			Assert.AreEqual( 0, doc.FontTable[ 01 ].CharSet );
			Assert.AreEqual( 1252, doc.FontTable[ 1 ].CodePage );
			Assert.AreEqual( "Verdana", doc.FontTable[ 1 ].Name );

			Assert.AreEqual( RtfFontKind.Roman, doc.FontTable[ 2 ].Kind );
			Assert.AreEqual( RtfFontPitch.Variable, doc.FontTable[ 2 ].Pitch );
			Assert.AreEqual( 2, doc.FontTable[ 2 ].CharSet );
			Assert.AreEqual( 42, doc.FontTable[ 2 ].CodePage );
			Assert.AreEqual( "Symbol", doc.FontTable[ 2 ].Name );

			Assert.AreSame( doc.DefaultFont, doc.FontTable[ 1 ] );

			Assert.AreEqual( 4, doc.ColorTable.Count );
			Assert.AreEqual( RtfColor.Black, doc.ColorTable[ 0 ] );
			Assert.AreEqual( RtfColor.Black, doc.ColorTable[ 1 ] );
			Assert.AreEqual( RtfColor.White, doc.ColorTable[ 2 ] );
			Assert.AreEqual( new RtfColor( 10, 20, 30 ), doc.ColorTable[ 3 ] );

			Assert.AreEqual( 2, doc.VisualContent.Count );
			Assert.AreEqual( RtfVisualKind.Text, doc.VisualContent[ 0 ].Kind );
			Assert.AreEqual( "Hellou RTF Wörld", ((IRtfVisualText)doc.VisualContent[ 0 ]).Text );
			Assert.AreEqual( "Verdana", ((IRtfVisualText)doc.VisualContent[ 0 ]).Format.Font.Name );
			Assert.AreEqual( 36, ((IRtfVisualText)doc.VisualContent[ 0 ]).Format.FontSize );
			Assert.AreEqual( RtfVisualKind.Break, doc.VisualContent[ 1 ].Kind );
			Assert.AreEqual( RtfVisualBreakKind.Paragraph, ((IRtfVisualBreak)doc.VisualContent[ 1 ]).BreakKind );

			Assert.AreEqual( 5, doc.UserProperties.Count );
			Assert.AreEqual( "created", doc.UserProperties[ 0 ].Name );
			Assert.AreEqual( RtfPropertyKind.Date, doc.UserProperties[ 0 ].PropertyKind );
			Assert.AreEqual( "2008-05-23", doc.UserProperties[ 0 ].StaticValue );
			Assert.IsNull( doc.UserProperties[ 0 ].LinkValue );
			Assert.AreEqual( "a link", doc.UserProperties[ 4 ].LinkValue );

			IRtfDocumentInfo info = doc.DocumentInfo;
			Assert.AreEqual( 2, info.Version );
			Assert.AreEqual( 3, info.Revision );
			Assert.AreEqual( 1, info.NumberOfPages );
			Assert.AreEqual( 3, info.NumberOfWords );
			Assert.AreEqual( 16, info.NumberOfCharacters );
			Assert.AreEqual( 314, info.Id );
			Assert.AreEqual( 17, info.EditingTimeInMinutes );
			Assert.AreEqual( "Not really important", info.Title );
			Assert.AreEqual( "RTF parsing", info.Subject );
			Assert.AreEqual( "John Doe", info.Author );
			Assert.AreEqual( "John Doe's boss", info.Manager );
			Assert.AreEqual( "Itenso GmbH", info.Company );
			Assert.AreEqual( "Foo Bar", info.Operator );
			Assert.AreEqual( "Development", info.Category );
			Assert.AreEqual( "RTF, Parser, Interpreter", info.Keywords );
			Assert.AreEqual( "a testing document", info.Comment );
			Assert.AreEqual( "with more commentary", info.DocumentComment );
			Assert.AreEqual( "http://wwww.nowhere.com/foo/bar", info.HyperLinkbase );
			Assert.AreEqual( Time( "2008.05.23-17:55:12" ), info.CreationTime );
			Assert.AreEqual( Time( "2008.05.23-18:01:00" ), info.RevisionTime );
			Assert.AreEqual( Time( "2008.05.23-17:59:00" ), info.PrintTime );
			Assert.AreEqual( Time( "2008.05.23-18:00:00" ), info.BackupTime );
		} // DocumentBuilderTest

		// ----------------------------------------------------------------------
		private static DateTime Time( string time )
		{
			return DateTime.ParseExact( time, "yyyy.MM.dd-HH:mm:ss", CultureInfo.InvariantCulture );
		} // Time

		// ----------------------------------------------------------------------
		[Test]
		public void UnicodeSupportTest()
		{
			const RtfParserListenerLogger parserLogger = null;
			//parserLogger = new RtfParserListenerLogger();
			IRtfGroup rtfStructure =
				RtfParserTool.Parse( GetTestResource( "RtfInterpreterTest_2.rtf" ), parserLogger );
			Assert.IsNotNull( rtfStructure );

			const RtfInterpreterListenerLogger interpreterLogger = null;
			//interpreterLogger = new RtfInterpreterListenerLogger();

			RtfTextConverter textConverter = new RtfTextConverter();

			RtfInterpreterTool.Interpret( rtfStructure, textConverter, interpreterLogger );

			string plainText = textConverter.PlainText;
			AssertEqualLines( "unicode support:",
				new StreamReader( GetTestResource( "RtfInterpreterTest_2.txt" ), RtfSpec.AnsiEncoding ),
				new StringReader( plainText ) );
		} // UnicodeSupportTest

		// ----------------------------------------------------------------------
		[Test]
		public void ExtractPlainTextTest()
		{
			const RtfParserListenerLogger parserLogger = null;
			//parserLogger = new RtfParserListenerLogger();
			IRtfGroup rtfStructure =
				RtfParserTool.Parse( GetTestResource( "RtfInterpreterTest_0.rtf" ), parserLogger );
			Assert.IsNotNull( rtfStructure );

			const RtfInterpreterListenerLogger interpreterLogger = null;
			//interpreterLogger = new RtfInterpreterListenerLogger();

		    var rtfString =
		        @"{\rtf1\sste16000\ansi\deflang1033\ftnbj\uc1\deff0 {\fonttbl{\f0 \fnil Verdana;}{\f1 \fnil \fcharset0 Verdana;}} {\colortbl;\red0\green0\blue0;} {\stylesheet{\f0\fs28 Normal;}{\cs1 Default Paragraph Font;}} {\*\revtbl{Unknown;}} \paperw12240\paperh15840\margl1800\margr1800\margt1440\margb1440\headery720\footery720\nogrowautofit\deftab720\formshade\fet4\aendnotes\aftnnrlc\pgbrdrhead\pgbrdrfoot \sectd\pgwsxn12240\pghsxn15840\marglsxn1800\margrsxn1800\margtsxn1440\margbsxn1440\headery720\footery720\sbkpage\pgncont\pgndec \plain\plain\f0\fs24\plain\f0\fs24\plain\f1\fs28\lang1033\hich\f1\dbch\f1\loch\f1\cf2\fs28 test again and again and again }";

		    var rtfString2 =
		        @"{\rtf1\sste16000\ansi\deflang1033\ftnbj\uc1\deff0 {\fonttbl{\f0 \f \fcharset0 Arial;}} {\colortbl ;\red255\green255\blue255 ;\red0\green0\blue0 ;\red0\green0\blue0 ;} {\stylesheet{\f0\fs24 Normal;}{\cs1 Default Paragraph Font;}} {\*\revtbl{Unknown;}} \paperw12240\paperh15840\margl1800\margr1800\margt1440\margb1440\headery720\footery720\htmautsp1\nogrowautofit\deftab720\formshade\fet4\aendnotes\aftnnrlc\pgbrdrhead\pgbrdrfoot \sectd\pgwsxn12240\pghsxn15840\marglsxn1800\margrsxn1800\margtsxn1440\margbsxn1440\headery720\footery720\sbkpage\pgncont\pgndec \plain\plain\f0\fs24\ltrpar\ql\plain\f0\fs24\plain\f0\fs24\lang1033\hich\f0\dbch\f0\loch\f0\cf2\fs24\ltrch this is my cover changed in bulk\plain\f0\fs24\par and then in text\par }";

		    var rtfString3 =
		        @"{\rtf1\sste16000\ansi\deflang1033\ftnbj\uc1\deff0 {\fonttbl{\f0 \fnil Arial;}{\f1 \fnil \fcharset0 Arial;}} {\colortbl ;\red0\green0\blue0 ;\red255\green255\blue255 ;\red0\green0\blue0 ;\red255\green0\blue0 ;\red0\green0\blue0 ;} {\stylesheet{\f0\fs24 Normal;}{\cs1 Default Paragraph Font;}} {\*\revtbl{Unknown;}} \paperw12240\paperh15840\margl1800\margr1800\margt1440\margb1440\headery720\footery720\nogrowautofit\deftab720\formshade\fet4\aendnotes\aftnnrlc\pgbrdrhead\pgbrdrfoot \sectd\pgwsxn12240\pghsxn15840\marglsxn1800\margrsxn1800\margtsxn1440\margbsxn1440\headery720\footery720\sbkpage\pgncont\pgndec \plain\hich\f0\dbch\f0\loch\f0\cf1\fs24\pard\lang1033\hich\f1\dbch\f1\loch\f1 12:24  v3465        AP-APTN-1630: Turkey Syria Wednesday, 3 October 2012\par  STORY:Turkey Syria- Shell fired from Syria kills three in Turkey LENGTH: 00:40\tab  FIRST RUN: 1530 RESTRICTIONS: No Access Turkey/ROJ TV TYPE: Natsound SOURCE: Anadolu Agency STORY NUMBER: 861347\par  DATELINE: Sanliurfa - 3 Oct 2012 LENGTH: 00:40\tab\par \par \par   \cf4\b\protect [Duration: 07:06]\plain\f1\fs24  \par \par \par  SHOTLIST:\par  1. Wide of smoke rising, policemen run for cover and surround wounded officer lying on ground  2. Wide of street, smoke seen rising from the distance 3. Tracking view of wounded person in a car which pulls away, policemen seen in background surrounding  injured officer lying on the ground 4. Mid plainclothes officers leading crowd and media away from injured officer 5. Wide of street with smoke seen, people running along\par  STORYLINE:\par  A Turkish official said on Wednesday that a shell fired from Syria had landed on a house in neighbouring Turkey, killing at least three people, including a 6-year-old boy.\par  Abdulhakim Ayhan, the mayor of the Turkish town of Akcakale which is along the Syrian border, told Turkey's state-owned Anadolu Agency that the boy and a woman were among the dead in Wednesday's shelling.\par  Anadolu Agency reported angry townspeople marched to the mayor's office to protest against the deaths.\par  Video from the Anadolu agency purportedly also showed at least one policeman was also injured in the incident.\par  The village was also hit by a mortar shell on Friday, though no-one was injured on that occasion.\par  The Turkish Foreign Minister Ahmet Davutoglu called an emergency meeting to discuss developments, after receiving information on the incident from Turkish Armed Forces.\par  ===========================================================\par  Clients are reminded:  (i) to check the terms of their licence agreements for use of content outside news programming and that further advice and assistance can be obtained from the AP Archive on: Tel +44 (0) 20 7482 7482 Email: infoaparchive.com (ii)  they should check with the applicable collecting society in their Territory regarding the clearance of any sound recording or performance included within the AP Television News service  (iii) they have editorial responsibility for the use of all and any  content included within the AP Television News service and for libel, privacy, compliance and third party rights applicable to their Territory.\par  APTN\par \par \par \tab    \par \tab    (Copyright 2012 by The Associated Press.  All Rights Reserved.)\par \tab    AP-NY-10-03-12 1224EDT\par   \par \fs18\par \ql\lang1033\hich\f0\dbch\f0\loch\f0\cf1\fs24\plain\f1\fs24\par }";

			RtfTextConverter textConverter = new RtfTextConverter();
			//RtfInterpreterTool.Interpret( rtfStructure, textConverter, interpreterLogger );
            RtfInterpreterTool.Interpret(rtfString3, textConverter, interpreterLogger);

			string plainText = textConverter.PlainText;
			Assert.AreEqual( "Hello RTF World\r\n", plainText );
		} // ExtractPlainTextTest

		// ----------------------------------------------------------------------
		[Test]
		public void DocumentRecognitionTest()
		{
			IterateResourceTestCases( "", "rtf", true );
		} // DocumentRecognitionTest

		// ----------------------------------------------------------------------
		protected override void DoTest( string kind, Stream testRes, string testCaseName )
		{
			const RtfParserListenerLogger parserLogger = null;
			//parserLogger = new RtfParserListenerLogger();

			const RtfInterpreterListenerLogger interpreterLogger = null;
			//interpreterLogger = new RtfInterpreterListenerLogger();

			RtfTextConverter textConverter = new RtfTextConverter();

			RtfInterpreterListenerDocumentBuilder docBuilder =
				new RtfInterpreterListenerDocumentBuilder();

			RtfInterpreterTool.Interpret( RtfParserTool.Parse( testRes, parserLogger ),
				interpreterLogger, textConverter, docBuilder );

			string plainText = textConverter.PlainText;
			Assert.IsFalse( string.IsNullOrEmpty( plainText ) );
			Assert.IsNotNull( docBuilder.Document );

			string testName = testCaseName.Substring( 0, testCaseName.Length - 4 );
			//string unicode = "バージョンアップ注文書（銀行振込）";
			Encoding enc;
			switch ( testName )
			{
				case "RtfInterpreterTest_12":
				case "RtfInterpreterTest_13":
				case "RtfInterpreterTest_14":
				case "RtfInterpreterTest_16":
				case "RtfInterpreterTest_17":
				case "RtfInterpreterTest_18":
				case "RtfInterpreterTest_19":
				case "RtfInterpreterTest_21":
					enc = Encoding.Unicode;
					break;
				default:
					enc = RtfSpec.AnsiEncoding;
					break;
			}
			string referenceResName = testName + ".txt";
			/*
			if ( "RtfInterpreterTest_21".Equals( testName ) )
			{
				using ( Stream s = new FileStream( @"w:\temp\rtf\RtfInterpreterTest_21.txt", FileMode.Create ) )
				{
					using ( TextWriter w = new StreamWriter( s, enc ) )
					{
						w.Write( plainText );
					}
				}
			}
			//*/
// ReSharper disable ConditionIsAlwaysTrueOrFalse
			if ( plainText == null )
// ReSharper restore ConditionIsAlwaysTrueOrFalse
// ReSharper disable HeuristicUnreachableCode
			{
				plainText = string.Empty;
			}
// ReSharper restore HeuristicUnreachableCode
			AssertEqualLines( "document recognition: " + testCaseName + ":",
				new StreamReader( GetTestResource( referenceResName ), enc ),
				new StringReader( plainText ) );
		} // DoTest

	} // class RtfInterpreterTest
	// ReSharper restore ExpressionIsAlwaysNull

} // namespace Itenso.Rtf.InterpreterTests
// -- EOF -------------------------------------------------------------------
