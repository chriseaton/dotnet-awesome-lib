/********************************************
 * MIT License
 * (c) Christopher Eaton, 2012
 * https://gitlab.com/chriseaton/awesome-lib
 ********************************************
 * CODE MODIFIED FROM:  * CODE MODIFIED FROM: http://madskristensen.net/post/A-realtime-currency-exchange-class-in-C.aspx
 * Originally created by: BlueLaser05, 2007
 * License: Code Project Open License - http://www.codeproject.com/info/cpol10.aspx
 ************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace Awesome.Library.Utilities {

	public enum CreditCardType {
		Unknown,
		Visa,
		MasterCard,
		Discover,
		AmericanExpress,
		Switch,
		Solo,
		DinersClub,
		enRoute,
		JCB
	}

	public class CreditCards {

		private const string cardRegex = "^(?:(?<Visa>4\\d{3})|(?<MasterCard>5[1-5]\\d{2})|(?<Discover>6011)|(?<DinersClub>(?:3[68]\\d{2})|(?:30[0-5]\\d))|(?<Amex>3[47]\\d{2}))([ -]?)(?(DinersClub)(?:\\d{6}\\1\\d{4})|(?(Amex)(?:\\d{6}\\1\\d{5})|(?:\\d{4}\\1\\d{4}\\1\\d{4})))$";

		public static bool IsValidNumber( string cardNumber ) {
			cardNumber = cardNumber.Trim();
			Regex cardTest = new Regex( cardRegex );
			//Determine the card type based on the number
			CreditCardType cardType = GetCardTypeFromNumber( cardNumber );
			//Call the base version of IsValidNumber and pass the 
			//number and card type
			if ( IsValidNumber( cardNumber, cardType ) )
				return true;
			else
				return false;
		}

		public static bool IsValidNumber( string cardNumber, CreditCardType cardType ) {
			cardNumber = cardNumber.Trim();
			//Create new instance of Regex comparer with our 
			//credit card regex pattern
			Regex cardTest = new Regex( cardRegex );

			//Make sure the supplied number matches the supplied
			//card type
			if ( cardTest.Match( cardNumber ).Groups[cardType.ToString()].Success ) {
				//If the card type matches the number, then run it
				//through Luhn's test to make sure the number appears correct
				if ( PassesLuhnTest( cardNumber ) )
					return true;
				else
					//The card fails Luhn's test
					return false;
			} else
				//The card number does not match the card type
				return false;
		}

		public static CreditCardType GetCardTypeFromNumber( string cardNumber ) {
			cardNumber = cardNumber.Trim();
			//Create new instance of Regex comparer with our
			//credit card regex pattern
			Regex cardTest = new Regex( cardRegex );
			//Compare the supplied card number with the regex
			//pattern and get reference regex named groups
			GroupCollection gc = cardTest.Match( cardNumber ).Groups;
			//Compare each card type to the named groups to 
			//determine which card type the number matches
			if ( gc[CreditCardType.AmericanExpress.ToString()].Success ) {
				return CreditCardType.AmericanExpress;
			} else if ( gc[CreditCardType.MasterCard.ToString()].Success ) {
				return CreditCardType.MasterCard;
			} else if ( gc[CreditCardType.Visa.ToString()].Success ) {
				return CreditCardType.Visa;
			} else if ( gc[CreditCardType.Discover.ToString()].Success ) {
				return CreditCardType.Discover;
			} else {
				// AMEX -- 34 or 37 -- 15 length
				if ( ( Regex.IsMatch( cardNumber, "^(34|37)" ) ) && ( 15 == cardNumber.Length ) )
					return CreditCardType.AmericanExpress;
				// MasterCard -- 51 through 55 -- 16 length
				else if ( Regex.IsMatch( cardNumber, "^(51|52|53|54|55)" ) && ( 16 == cardNumber.Length ) )
					return CreditCardType.MasterCard;
				// VISA -- 4 -- 13 and 16 length
				else if ( Regex.IsMatch( cardNumber, "^(4)" ) && ( 13 == cardNumber.Length || 16 == cardNumber.Length ) )
					return CreditCardType.Visa;
				// Diners Club -- 300-305, 36 or 38 -- 14 length
				else if ( Regex.IsMatch( cardNumber, "^(300|301|302|303|304|305|36|38)" ) && ( 14 == cardNumber.Length ) )
					return CreditCardType.DinersClub;
				// enRoute -- 2014,2149 -- 15 length
				else if ( Regex.IsMatch( cardNumber, "^(2014|2149)" ) && ( 15 == cardNumber.Length ) )
					return CreditCardType.enRoute;
				// Discover -- 6011 -- 16 length
				else if ( Regex.IsMatch( cardNumber, "^(6011)" ) && ( 16 == cardNumber.Length ) )
					return CreditCardType.Discover;
				// JCB -- 3 -- 16 length
				else if ( Regex.IsMatch( cardNumber, "^(3)" ) && ( 16 == cardNumber.Length ) )
					return CreditCardType.JCB;
				// JCB -- 2131, 1800 -- 15 length
				else if ( Regex.IsMatch( cardNumber, "^(2131|1800)" ) && ( 15 == cardNumber.Length ) )
					return CreditCardType.JCB;
				//Card type is not supported by our system, return unknown
				return CreditCardType.Unknown;
			}
		}

		public static string GetCardTestNumber( CreditCardType cardType ) {
			//According to PayPal, the valid test numbers that should be used
			//for testing card transactions are:
			//Credit Card Type              Credit Card Number
			//American Express              378282246310005
			//American Express              371449635398431
			//American Express Corporate    378734493671000
			//Diners Club                   30569309025904
			//Diners Club                   38520000023237
			//Discover                      6011111111111117
			//Discover                      6011000990139424
			//MasterCard                    5555555555554444
			//MasterCard                    5105105105105100
			//Visa                          4111111111111111
			//Visa                          4012888888881881
			//Src: https://www.paypal.com/en_US/vhelp/paypalmanager_help/credit_card_numbers.htm
			//Credit: Scott Dorman, http://www.geekswithblogs.net/sdorman
			//Return bogus CC number that passes Luhn and format tests
			switch ( cardType ) {
				case CreditCardType.AmericanExpress:
					return "3782 822463 10005";
				case CreditCardType.Discover:
					return "6011 1111 1111 1117";
				case CreditCardType.MasterCard:
					return "5105 1051 0510 5100";
				case CreditCardType.Visa:
					return "4111 1111 1111 1111";
				default:
					return null;
			}
		}

		public static bool PassesLuhnTest( string cardNumber ) {
			//Clean the card number- remove dashes and spaces
			cardNumber = cardNumber.Replace( "-", "" ).Replace( " ", "" );
			//Convert card number into digits array
			int[] digits = new int[cardNumber.Length];
			for ( int len = 0; len < cardNumber.Length; len++ ) {
				digits[len] = Int32.Parse( cardNumber.Substring( len, 1 ) );
			}
			//Luhn Algorithm
			//Adapted from code availabe on Wikipedia at
			//http://en.wikipedia.org/wiki/Luhn_algorithm
			int sum = 0;
			bool alt = false;
			for ( int i = digits.Length - 1; i >= 0; i-- ) {
				int curDigit = digits[i];
				if ( alt ) {
					curDigit *= 2;
					if ( curDigit > 9 ) {
						curDigit -= 9;
					}
				}
				sum += curDigit;
				alt = !alt;
			}
			//If Mod 10 equals 0, the number is good and this will return true
			return sum % 10 == 0;
		}

	}

}
