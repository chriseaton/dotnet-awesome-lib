/********************************************
 * MIT License
 * (c) Christopher Eaton, 2012
 * https://github.com/chriseaton/dotnet-awesome-lib
 ********************************************/
using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace Awesome.Library.Cryptography {

    /// <summary>
    /// ROT13 Quick Un-secure Cryptographic Algorithm
    /// </summary>
    public static class ROT13Crypto {

        /// <summary>  
        /// Encodes/Decodes text using the ROT13 algorithm  
        /// *WARNING* Very Weak Encryption
        /// </summary>  
        /// <param name="InputText">The text to be decoded or encoded</param>  
        /// <returns>The encoded or decoded string result</returns>  
        public static string ROT13Encode(string InputText) {
            int i;
            char CurrentCharacter;
            int CurrentCharacterCode;
            string EncodedText = "";
            //Iterate through the length of the input parameter  
            for (i = 0; i < InputText.Length; i++) {
                //Convert the current character to a char  
                CurrentCharacter = System.Convert.ToChar(InputText.Substring(i, 1));
                //Get the character code of the current character  
                CurrentCharacterCode = (int)CurrentCharacter;
                //Modify the character code of the character, - this  
                //so that "a" becomes "n", "z" becomes "m", "N" becomes "Y" and so on  
                if (CurrentCharacterCode >= 97 && CurrentCharacterCode <= 109) {
                    CurrentCharacterCode = CurrentCharacterCode + 13;
                } else
                    if (CurrentCharacterCode >= 110 && CurrentCharacterCode <= 122) {
                        CurrentCharacterCode = CurrentCharacterCode - 13;
                    } else
                        if (CurrentCharacterCode >= 65 && CurrentCharacterCode <= 77) {
                            CurrentCharacterCode = CurrentCharacterCode + 13;
                        } else
                            if (CurrentCharacterCode >= 78 && CurrentCharacterCode <= 90) {
                                CurrentCharacterCode = CurrentCharacterCode - 13;
                            }
                //Add the current character to the string to be returned  
                EncodedText = EncodedText + (char)CurrentCharacterCode;
            }
            return EncodedText;
        }

    }

}
