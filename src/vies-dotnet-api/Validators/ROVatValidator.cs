/*
   Copyright 2017-2023 Adrian Popescu.
   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at
       http://www.apache.org/licenses/LICENSE-2.0
   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Padi.Vies.Validators;

/// <summary>
/// 
/// </summary>
[SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
public sealed class RoVatValidator : VatValidatorAbstract
{
    private const string REGEX_PATTERN =@"^[0-9]{2,10}$";
    private const string COUNTRY_CODE = nameof(EuCountryCode.RO);

    private static readonly Regex _regex = new(REGEX_PATTERN, RegexOptions.Compiled, TimeSpan.FromSeconds(5));

    private static readonly int[] Multipliers = { 7, 5, 3, 2, 1, 7, 5, 3, 2 };

    public RoVatValidator()
    {
        this.Regex = _regex;
        CountryCode = COUNTRY_CODE;
    }
        
    protected override VatValidationResult OnValidate(string vat)
    {
        var end = vat.Length - 1;
            
        var controlDigit = vat[end].ToInt();

        var slice = vat.Slice(0, end);

        vat = slice.PadLeft(9, '0');

        var sum = vat.Sum(Multipliers);

        var checkDigit = sum * 10 % 11;
                
        if (checkDigit == 10)
        {
            checkDigit = 0;
        }

        var isValid = checkDigit == controlDigit;
        return !isValid 
            ? VatValidationResult.Failed("Invalid RO vat: checkValue") 
            : VatValidationResult.Success();
    }
}