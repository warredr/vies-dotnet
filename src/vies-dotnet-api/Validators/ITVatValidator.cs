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
using System.Globalization;
using System.Text.RegularExpressions;

namespace Padi.Vies.Validators;

[SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
internal sealed class ItVatValidator : VatValidatorAbstract
{
    private const string REGEX_PATTERN =@"^\d{11}$";
    private const string COUNTRY_CODE = nameof(EuCountryCode.IT);

    private static readonly Regex _regex = new(REGEX_PATTERN, RegexOptions.Compiled, TimeSpan.FromSeconds(5));

    private static readonly int[] Multipliers = {1, 2, 1, 2, 1, 2, 1, 2, 1, 2};

    public ItVatValidator()
    {
        this.Regex = _regex;
        CountryCode = COUNTRY_CODE;
    }
        
    protected override VatValidationResult OnValidate(string vat)
    {
        // The last three digits are the issuing office, and cannot exceed more 201
        if (int.Parse(vat.Slice(0, 7), NumberStyles.Integer, CultureInfo.InvariantCulture) == 0)
        {
            return VatValidationResult.Failed("");
        }

        var temp = int.Parse(vat.Slice(7, 3),NumberStyles.Integer, CultureInfo.InvariantCulture);

        if ((temp < 1 || temp > 201) && temp != 999 && temp != 888)
        {
            return VatValidationResult.Failed("");
        }

        var index = 0;
        var sum = 0;
        foreach (var m in Multipliers)
        {
            temp = vat[index++].ToInt() * m;
            sum += temp > 9 
                ? (int) Math.Floor(temp / 10D) + temp % 10 
                : temp;
        }

        var checkDigit = 10 - sum % 10;
            
        if (checkDigit > 9)
        {
            checkDigit = 0;
        }

        var isValid = checkDigit == vat[10].ToInt();
        return !isValid 
            ? VatValidationResult.Failed("Invalid IT vat: checkValue") 
            : VatValidationResult.Success();
    }
}