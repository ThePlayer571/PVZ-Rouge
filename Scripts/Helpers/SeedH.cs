using System;
using System.Globalization;

namespace TPL.PVZR.Helpers.New
{
    public static class SeedHelper 
    {
        /// <summary>
        /// 校验输入是否为合法的 8 位 16 进制字符串
        /// </summary>
        public static bool ValidateInput(string input)
        {
            if (string.IsNullOrEmpty(input) || input.Length != 8)
                return false;

            foreach (char c in input)
            {
                if (!Uri.IsHexDigit(c)) return false;
            }

            return true;
        }
        

        /// <summary>
        /// 将合法的 8 位 16 进制字符串转换为 ulong 值
        /// </summary>
        public static ulong ParseFrom(string input)
        {
            if (!ValidateInput(input))
                throw new FormatException("输入必须是8位16进制字符");

            return ulong.Parse(input, NumberStyles.HexNumber);
        }
    }
}