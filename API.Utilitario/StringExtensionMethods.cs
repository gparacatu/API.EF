using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace AppPromotora.Api.Utilitario
{

    public static class StringExtensionMethods
    {

       
        public static string Phonetized(this string text)
        {
            return text.Phonetized(true);
        }

        public static string Phonetized(this string text, bool mantemVogais)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            //Devemos retirar os caracteres especiais e converter todas as letras para Maiúsculo
            //Eliminamos todos os acentos
            StringBuilder sb = new StringBuilder(text.Trim().ToUpper().RemoveAcentos(false));

            //Substituimos
            sb.Replace("Y", "I");
            sb.Replace("BR", "B").Replace("BL", "B").Replace("PH", "F");
            sb.Replace("GR", "G").Replace("GL", "G").Replace("MG", "G");
            sb.Replace("NG", "G").Replace("RG", "G").Replace("GE", "J");
            sb.Replace("GI", "J").Replace("RJ", "J").Replace("MJ", "J");
            sb.Replace("NJ", "J").Replace("CE", "S").Replace("CI", "S");
            sb.Replace("CH", "S").Replace("CT", "T").Replace("CS", "S");
            sb.Replace("Q", "K").Replace("CA", "K").Replace("CO", "K");
            sb.Replace("CU", "K").Replace("C", "K").Replace("LH", "H");
            sb.Replace("RM", "SM").Replace("N", "M").Replace("GM", "M");
            sb.Replace("MD", "M").Replace("NH", "N").Replace("PR", "P");
            sb.Replace("X", "S").Replace("TS", "S").Replace("C", "S");
            sb.Replace("Z", "S").Replace("RS", "S").Replace("TR", "T");
            sb.Replace("TL", "T").Replace("LT", "T").Replace("RT", "T");
            sb.Replace("ST", "T").Replace("W", "V");

            //Eliminamos as terminações S, Z, R, R, M, N, AO e L
            int tam = sb.Length - 1;
            if (tam > -1)
                if (sb[tam] == 'S' || sb[tam] == 'Z' || sb[tam] == 'R' || sb[tam] == 'M' || sb[tam] == 'N' || sb[tam] == 'L')
                    sb.Remove(tam, 1);
            tam = sb.Length - 2;

            if (tam > -1)
                if (sb[tam] == 'A' && sb[tam + 1] == 'O')
                    sb.Remove(tam, 2);

            //Substituimos L por R e Ç por S;
            sb.Replace("Ç", "S").Replace("L", "R");

            //O BuscaBr diz para eliminamos todas as vogais e o H, 
            //porém ao implementar notamos que não seria necessário 
            //eliminarmos as vogais, isso dificultaria muito a busca dos dados "pós BuscaBR"
            if (!mantemVogais)
                sb.Replace("A", "").Replace("E", "").Replace("I", "").Replace("O", "").Replace("U", "");
            sb.Replace("H", "");

            //Eliminamos todas as letras em duplicidade;
            StringBuilder frasesaida = new StringBuilder();
            if (sb.Length > 0)
            {
                frasesaida.Append(sb[0]);
                for (int i = 1; i <= sb.Length - 1; i += 1)
                    if (frasesaida[frasesaida.Length - 1] != sb[i] || char.IsDigit(sb[i]))
                        frasesaida.Append(sb[i]);
            }

            return frasesaida.ToString();
        }

        public static bool IsBase64String(this string s)
        {
            s = s.Trim();
            return (s.Length % 4 == 0) && Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);

        }

        public static string HTMLCleanDecode(this string text)
        {
            return HttpUtility.HtmlDecode(text).Replace("\r", "").Replace("\t", "").Replace("\n", "").Trim();
        }


        public static string RemoveAcentos(this string text)
        {
            return text.RemoveAcentos(true);
        }

        /// <summary>
        /// alem dos acentos exclui também caracteres não ASCII (fora da faixa de 20 a 126)
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string RemoveAcentosASCII(this string text)
        {

            return Regex.Replace(text.RemoveAcentos(true), @"[^\u0020-\u007E]", " ");
        }

        public static string RemoveAcentos(this string text, bool includeCedilla)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            text = Regex.Replace(text, "[áàãâä]", "a");
            text = Regex.Replace(text, "[ÁÀÃÂÄ]", "A");
            text = Regex.Replace(text, "[èéêë]", "e");
            text = Regex.Replace(text, "[ÈÉÊË]", "E");
            text = Regex.Replace(text, "[îìïí]", "i");
            text = Regex.Replace(text, "[ÎÌÏÍ]", "I");
            text = Regex.Replace(text, "[óòõöô]", "o");
            text = Regex.Replace(text, "[ÓÒÕÖÔ]", "O");
            text = Regex.Replace(text, "[úùüû]", "u");
            text = Regex.Replace(text, "[ÚÙÜÛ]", "U");
            text = text.Replace("–", "-");

            if (includeCedilla)
                text = text.Replace("ç", "c").Replace("Ç", "C");

            return text;
        }

        public static string RemovePontuacao(this string text)
        {
            return text.RemovePontuacao(true);
        }

        public static string RemoveAcentosForAddress(this string text)
        {
            return text.RemoveAcentos().RemovePontuacao(false, true);
        }

        public static string RemovePontuacao(this string text, bool removeEmpty, bool forAddress = false)
        {
            if (text == null)
                return string.Empty;

            if (forAddress)
            {
                text = Regex.Replace(text, @"['´""]", string.Empty);
                text = Regex.Replace(text, "-", " ");
            }
            else
            {
                text = text.Trim();
                text = Regex.Replace(text, @"[-_/\.,:;()]", string.Empty);
            }

            if (removeEmpty)
                text = text.Replace(" ", string.Empty);
            /*
            text = text.Replace("-", "");
            text = text.Replace("_", "");
            text = text.Replace("/", "");
            text = text.Replace(@"\", "");
            text = text.Replace(".", "");
            text = text.Replace(",", "");
            text = text.Replace(":", "");
            text = text.Replace(";", "");
            text = text.Replace("(", "");
            text = text.Replace(")", ""); */

            return text;
        }

        //---------------------------------------------------------------------------
        // Base64
        //---------------------------------------------------------------------------
        public static string ToBase64String(this string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            byte[] buffer = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(buffer);
        }

        public static string ToBase64String(this byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            return Convert.ToBase64String(buffer);
        }

        public static string FromBase64String(this string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            byte[] buffer = Convert.FromBase64String(text);
            return Encoding.UTF8.GetString(buffer);
        }


        //---------------------------------------------------------------------------
        // compressão / descompressão
        //---------------------------------------------------------------------------
        public static string Compress(this string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            byte[] buffer = Encoding.UTF8.GetBytes(text);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
                {
                    gZipStream.Write(buffer, 0, buffer.Length);
                }

                memoryStream.Position = 0;

                byte[] compressed = new byte[memoryStream.Length];
                memoryStream.Read(compressed, 0, compressed.Length);

                byte[] gzBuffer = new byte[compressed.Length + 4];
                Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
                Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);

                return Convert.ToBase64String(gzBuffer);
            }
        }

        public static string Decompress(this string compressedText)
        {
            if (compressedText == null)
                throw new ArgumentNullException("compressedText");

            byte[] gzBuffer = Convert.FromBase64String(compressedText);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                int msgLength = BitConverter.ToInt32(gzBuffer, 0);
                memoryStream.Write(gzBuffer, 4, gzBuffer.Length - 4);

                byte[] buffer = new byte[msgLength];

                memoryStream.Position = 0;
                using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    gZipStream.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }
        }

        #region FORMATAÇÃO DE DADOS
        public static string FormataCpf(this string text)
        {
            return text.FormataCpf(0);
        }
        public static string FormataCpf(this string text, int Mascara)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            else
            {
                text = text.RemovePontuacao();

                if (text.Length < 11)
                    text = text.PadLeft(11, '0');
                else
                    text = text.Right(11);

                switch (Mascara)
                {
                    case 1: return string.Format("{0}.XXX.XXX-{1}", text.Substring(0, 3), text.Substring(9, 2));
                    case 2: return string.Format("{0}.***.***-{1}", text.Substring(0, 3), text.Substring(9, 2));
                    default: return Convert.ToUInt64(text).ToString(@"000\.000\.000\-00");
                }
            }
        }

        public static string FormataCnpj(this string text)
        {
            return text.FormataCnpj(0);
        }
        public static string FormataCnpj(this string text, int Mascara)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            else
            {
                text = text.RemovePontuacao();
                switch (Mascara)
                {
                    case 1: return string.Format("{0}.XXX.XXX/XXXX-{4}", text.Substring(0, 2), text.Substring(2, 3), text.Substring(5, 3), text.Substring(8, 4), text.Substring(12, 2));
                    case 2: return string.Format("{0}.***.***/****-{4}", text.Substring(0, 2), text.Substring(2, 3), text.Substring(5, 3), text.Substring(8, 4), text.Substring(12, 2));
                    default: return Convert.ToUInt64(text).ToString(@"00\.000\.000\/0000\-00");
                }
            }
        }

        public static string FormataDocumento(this string text, int Mascara = 0)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            else
            {
                text = text.RemovePontuacao();
                if (text.Length < 14)
                    return FormataCpf(text, Mascara);
                else
                    return FormataCnpj(text, Mascara);
            }
        }

        public static string FormataTelefone(this string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            else
            {
                if (text.Length == 8)
                {
                    return string.Format("{0}-{1}", text.Substring(0, 4), text.Substring(4, 4));
                }
                else
                    if (text.Length == 9)
                {
                    return string.Format("{0}-{1}", text.Substring(0, 5), text.Substring(5, 4));
                }
                else
                        if (text.Length == 10)
                {
                    return string.Format("({0}) {1}-{2}", text.Substring(0, 2), text.Substring(2, 4), text.Substring(6, 4));
                }
                else
                            if (text.Length == 11)
                {
                    if (text.Substring(0, 4).Equals("0800"))
                        return string.Format("({0}) {1}-{2}", text.Substring(0, 4), text.Substring(4, 3), text.Substring(7, 4));
                    else
                        return string.Format("({0}) {1}-{2}", text.Substring(0, 2), text.Substring(2, 5), text.Substring(7, 4));
                }
                else
                    return text;
            }
        }

        public static string FormataCep(this string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            else
            {
                if (text.Length < 8) return text;
                else return string.Format("{0}-{1}", text.Substring(0, 5), text.Substring(5, 3));
            }
        }

        public static string FormataToSql(this string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            else
            {
                var separador = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                var separadormilhares = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;

                return text.Replace(separadormilhares, string.Empty).Replace(separador, ".");
            }
        }

        //Formatação do cartão
        public static string FormataCartao(this string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            else
            {
                text = text.RemovePontuacao();
                if (text.Length < 16)
                    return text;
                else
                    return string.Format("{0} {1} {2} {3}", text.Substring(0, 4), text.Substring(4, 4), text.Substring(8, 4), text.Substring(12, 4));
            }
        }

        /// <summary>
        /// Formata o Cartão exibindo "**** ****" nos dois blocos centrais do cartão
        /// </summary>
        /// <param name="text">Número do Cartão</param>
        public static string FormataCartaoMask(this string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            else
            {
                text = text.RemovePontuacao();
                if (text.Length < 16)
                    return text;
                else
                    return string.Format("{0} **** **** {3}", text.Substring(0, 4), text.Substring(4, 4), text.Substring(8, 4), text.Substring(12, 4));
            }
        }

        public static string FormataAutorizacao(this string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            else
            {
                text = text.RemovePontuacao();
                if (text.Length < 10)
                    return text;
                else
                    return string.Format("{0} {1} {2} {3} {4}", text.Substring(0, 2), text.Substring(2, 2), text.Substring(4, 2), text.Substring(6, 2), text.Substring(8, 2));
            }
        }

        /// <summary>
        /// Formatar Carteira Nacional de Saúde (CNS)
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string FormataCNS(this string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            else
            {
                text = text.RemovePontuacao();
                if (text.Length < 15)
                    return text;
                else
                    return string.Format("{0} {1}", text.Substring(0, 11), text.Substring(11, 4));
            }
        }


        /// <summary>
        /// Formatar Declaração Nascido Vivo (DNV)
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string FormataDNV(this string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            else
            {
                text = text.RemovePontuacao();
                if (text.Length < 11)
                    return text;
                else
                    return string.Format("{0}-{1}-{2}", text.Substring(0, 2), text.Substring(2, 8), text.Substring(10, 1));
            }
        }

        public static string FormataNumeroConta(this string conta)
        {
            if (string.IsNullOrEmpty(conta)) return string.Empty;
            else
            {
                if (conta.Length < 12) return conta;
                return string.Format("{0}-{1}-{2}", conta.Substring(0, 4), conta.Substring(4, 4), conta.Substring(8, 4));
            }
        }


        #endregion FORMATAÇÃO DE DADOS

        #region CONVERSÕES
        public static decimal? ToDecimal(this string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                var separador = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
                decimal valor;
                if (decimal.TryParse(text.Replace(separador, string.Empty), out valor))
                    return valor;
                else
                    return null;
            }
            else return null;
        }

        public static double? ToDouble(this string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                var separador = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
                double valor;
                if (Double.TryParse(text.Replace(separador, string.Empty), out valor))
                    return valor;
                else
                    return null;
            }
            else return null;
        }

        public static string ToCurrency(this string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            else
            {
                decimal? valor = ToDecimal(text);
                if (valor.HasValue)
                {
                    return valor.Value.ToString("c" + System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalDigits);
                }
                else return string.Empty;
            }
        }

        public static short? ToInt16(this string text)
        {
            if (!String.IsNullOrEmpty(text))
            {
                short valor;
                if (Int16.TryParse(text, out valor))
                    return valor;
                else
                    return null;
            }
            else
                return null;
        }

        public static int? ToInt32(this string text)
        {
            if (!String.IsNullOrEmpty(text))
            {
                int valor;
                if (Int32.TryParse(text, out valor))
                    return valor;
                else
                    return null;
            }
            else
                return null;
        }

        public static long? ToInt64(this string text)
        {
            if (!String.IsNullOrEmpty(text))
            {
                long valor;
                if (Int64.TryParse(text, out valor))
                    return valor;
                else
                    return null;
            }
            else
                return null;
        }

        public static DateTime? ToDateTime(this string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                DateTime valor;
                if (DateTime.TryParse(text, out valor))
                    return valor;
                else
                    return null;
            }
            else return null;
        }
        public static DateTime ToDate(this string text)
        {
            try
            {
                if (text.Length == 8)
                {
                    return new DateTime(text.Substring(4, 4).ToInt16() ?? 0,
                                        text.Substring(2, 2).ToInt16() ?? 0,
                                        text.Substring(0, 2).ToInt16() ?? 0);
                }
                else
                {
                    return DateTime.Parse(text);
                }
            }
            catch
            {
                throw new Exception("Data inválida!");
            }
        }



        public static string HTMLtoText(string source)
        {
            if (string.IsNullOrEmpty(source)) return string.Empty;

            try
            {
                string result;

                result = source.Replace("\r", " ");
                result = result.Replace("\n", " ");
                result = result.Replace("\t", string.Empty);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                                                                      @"( )+", " ");

                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*head([^>])*>", "<head>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<( )*(/)( )*head( )*>)", "</head>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(<head>).*(</head>)", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*script([^>])*>", "<script>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<( )*(/)( )*script( )*>)", "</script>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<script>).*(</script>)", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*style([^>])*>", "<style>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<( )*(/)( )*style( )*>)", "</style>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(<style>).*(</style>)", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*td([^>])*>", "\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*br( )*>", "\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*li( )*>", "\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*div([^>])*>", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*tr([^>])*>", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*p([^>])*>", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<[^>]*>", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @" ", " ",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&bull;", " * ",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&lsaquo;", "<",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&rsaquo;", ">",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&trade;", "(tm)",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&frasl;", "/",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&lt;", "<",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&gt;", ">",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&copy;", "(c)",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&reg;", "(r)",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // http://hotwired.lycos.com/webmonkey/reference/special_characters/
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&(.{2,6});", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                result = result.Replace("\n", "\r");

                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)( )+(\r)", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\t)( )+(\t)", "\t\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\t)( )+(\r)", "\t\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)( )+(\t)", "\r\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)(\t)+(\r)", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)(\t)+", "\r\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                string breaks = "\r\r\r";
                string tabs = "\t\t\t\t\t";
                for (int index = 0; index < result.Length; index++)
                {
                    result = result.Replace(breaks, "\r\r");
                    result = result.Replace(tabs, "\t\t\t\t");
                    breaks = breaks + "\r";
                    tabs = tabs + "\t";
                }

                return result;
            }
            catch
            {
                return source;
            }
        }



        /// <summary>
        /// converter texto em HTML
        /// </summary>
        /// <param name="text">Texto a ser convertido</param>
        /// <param name="allow">Se pocessa elementos HTML presentes no texto </param>
        /// <param name="space">Se pocessa espaços (&nbsp;) presentes no texto </param>
        public static string TextToHTML(this string text)
        {
            return TextToHTML(text, false);
        }

        public static string TextToHTML(this string text, bool allow)
        {
            return TextToHTML(text, allow, false);
        }

        public static string TextToHTML(this string text, bool allow, bool space)
        {
            StringBuilder sb = new StringBuilder(text);
            if (!space)
                sb.Replace(" ", "&nbsp;");
            if (!allow)
            {
                sb.Replace("<", "&lt;");
                sb.Replace(">", "&gt;");
                sb.Replace("\"", "&quot;");
            }
            StringReader sr = new StringReader(sb.ToString());
            StringWriter sw = new StringWriter();
            while (sr.Peek() > -1)
            {
                string temp = sr.ReadLine();
                sw.Write(temp + "<br>");
            }
            return sw.GetStringBuilder().ToString();
        }

        #endregion CONVERSÕES

        #region VALIDAÇÕES

        public static bool ValidaEmail(this string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
    
        public static bool ValidaCnpj(this string cnpj)
        {
            int soma = 0, dig;

            string cnpjOriginal = cnpj.RemovePontuacao();
            if (cnpjOriginal.Length != 14) return false;

            string cnpjComparacao = cnpjOriginal.Substring(0, 12);

            char[] charCnpj = cnpjOriginal.ToCharArray();

            /* Primeira parte */
            for (int i = 0; i < 4; i++)
                if (charCnpj[i] - 48 >= 0 && charCnpj[i] - 48 <= 9)
                    soma += (charCnpj[i] - 48) * (6 - (i + 1));
            for (int i = 0; i < 8; i++)
                if (charCnpj[i + 4] - 48 >= 0 && charCnpj[i + 4] - 48 <= 9)
                    soma += (charCnpj[i + 4] - 48) * (10 - (i + 1));
            dig = 11 - (soma % 11);

            cnpjComparacao += (dig == 10 || dig == 11) ? "0" : dig.ToString();

            /* Segunda parte */
            soma = 0;
            for (int i = 0; i < 5; i++)
                if (charCnpj[i] - 48 >= 0 && charCnpj[i] - 48 <= 9)
                    soma += (charCnpj[i] - 48) * (7 - (i + 1));
            for (int i = 0; i < 8; i++)
                if (charCnpj[i + 5] - 48 >= 0 && charCnpj[i + 5] - 48 <= 9)
                    soma += (charCnpj[i + 5] - 48) * (10 - (i + 1));
            dig = 11 - (soma % 11);
            cnpjComparacao += (dig == 10 || dig == 11) ? "0" : dig.ToString();

            return cnpjOriginal == cnpjComparacao;
        }

        public static bool ValidaCpf(this string cpf)
        {
            //variáveis
            int digito1, digito2;
            int adicao = 0;

            string digito = "";
            string calculo = "";

            string cpfComparacao = cpf.RemovePontuacao();

            // Se o tamanho for < 11 entao retorna como inválido
            if (cpfComparacao.Length != 11) return false;

            // Pesos para calcular o primeiro número 
            int[] array1 = new int[] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            // Pesos para calcular o segundo número
            int[] array2 = new int[] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            // Caso coloque todos os numeros iguais
            if (Regex.Match(cpfComparacao, "0{11}|1{11}|2{11}|3{11}|4{11}|5{11}|6{11}|7{11}|8{11}|9{11}").Success) return false;

            // Calcula cada número com seu respectivo peso 
            for (int i = 0; i <= array1.GetUpperBound(0); i++)
                adicao += (array1[i] * Convert.ToInt32(cpfComparacao[i].ToString()));

            // Pega o resto da divisão 
            int resto = adicao % 11;

            if (resto == 1 || resto == 0)
                digito1 = 0;
            else
                digito1 = 11 - resto;

            adicao = 0;

            // Calcula cada número com seu respectivo peso 
            for (int i = 0; i <= array2.GetUpperBound(0); i++)
                adicao += (array2[i] * Convert.ToInt32(cpfComparacao[i].ToString()));

            // Pega o resto da divisão 
            resto = adicao % 11;

            if (resto == 1 || resto == 0)
                digito2 = 0;
            else
                digito2 = 11 - resto;

            calculo = digito1.ToString() + digito2.ToString();
            digito = cpfComparacao.Substring(9, 2);

            // Se os ultimos dois digitos calculados bater com 
            // os dois ultimos digitos do cpf entao é válido 
            return calculo == digito;
        }

        public static bool ValidaDocumento(this string documento)
        {
            string documentoComparacao = documento.RemovePontuacao();

            if (documentoComparacao.Length == 11) return documentoComparacao.ValidaCpf();
            else if (documentoComparacao.Length == 14) return documentoComparacao.ValidaCnpj();
            else return false;
        }

        public static bool ValidaEAN13(this string CodBar)
        {
            bool Result = false;
            CodBar = RetornaEAN13(CodBar);

            if (CodBar.Length == 13)
            {
                try
                {
                    string sl_CodIni = CodBar.Substring(0, CodBar.Length - 1);
                    int il_Soma = 0;
                    int il_SeqSom = 1;

                    for (int il_TodNum = 0; il_TodNum < sl_CodIni.Length; il_TodNum++)
                    {
                        il_Soma = il_Soma + (Convert.ToInt32(sl_CodIni.Substring(il_TodNum, 1)) * il_SeqSom);
                        if (il_SeqSom == 1)
                            il_SeqSom = 3;
                        else
                            il_SeqSom = 1;
                    }

                    if (il_Soma.ToString().Substring(il_Soma.ToString().Length - 1, 1) != "0")
                    {
                        if (CodBar.Substring(CodBar.Length - 1, 1).Equals((10 - Convert.ToInt32(il_Soma.ToString().Substring(il_Soma.ToString().Length - 1, 1))).ToString()))
                            Result = true;
                    }
                    else
                        if (Convert.ToInt32(CodBar.Substring(CodBar.Length - 1, 1)) == 0)
                        Result = true;
                }
                catch { }
            }

            return Result;
        }

        /// <summary>
        /// Validação da Carteira Nacional de Saúde
        /// </summary>
        /// <param name="CNS"></param>
        /// <returns></returns>
        public static bool ValidaCNS(this string CNS)
        {
            bool Result = false;
            float Soma;
            float Resto, DV;
            string Resultado = string.Empty;
            string PIS = string.Empty;

            if (CNS.Trim().Length != 15)
                return Result;

            PIS = CNS.SomenteNumeros();
            if (!CNS.Equals(PIS))
                return Result;

            if ((!CNS.Substring(0, 1).Equals("1")) && (!CNS.Substring(0, 1).Equals("2")))
                return Result;

            PIS = CNS.Substring(0, 11);

            Soma = (int.Parse(PIS.Substring(00, 1)) * 15) +
                   (int.Parse(PIS.Substring(01, 1)) * 14) +
                   (int.Parse(PIS.Substring(02, 1)) * 13) +
                   (int.Parse(PIS.Substring(03, 1)) * 12) +
                   (int.Parse(PIS.Substring(04, 1)) * 11) +
                   (int.Parse(PIS.Substring(05, 1)) * 10) +
                   (int.Parse(PIS.Substring(06, 1)) * 09) +
                   (int.Parse(PIS.Substring(07, 1)) * 08) +
                   (int.Parse(PIS.Substring(08, 1)) * 07) +
                   (int.Parse(PIS.Substring(09, 1)) * 06) +
                   (int.Parse(PIS.Substring(10, 1)) * 05);

            Resto = Soma % 11;
            DV = 11 - Resto;

            if (DV == 11)
                DV = 0;

            if (DV == 10)
            {
                Soma = (int.Parse(PIS.Substring(00, 1)) * 15) +
                       (int.Parse(PIS.Substring(01, 1)) * 14) +
                       (int.Parse(PIS.Substring(02, 1)) * 13) +
                       (int.Parse(PIS.Substring(03, 1)) * 12) +
                       (int.Parse(PIS.Substring(04, 1)) * 11) +
                       (int.Parse(PIS.Substring(05, 1)) * 10) +
                       (int.Parse(PIS.Substring(06, 1)) * 09) +
                       (int.Parse(PIS.Substring(07, 1)) * 08) +
                       (int.Parse(PIS.Substring(08, 1)) * 07) +
                       (int.Parse(PIS.Substring(09, 1)) * 06) +
                       (int.Parse(PIS.Substring(10, 1)) * 05) + 2;
                Resto = Soma % 11;
                DV = 11 - Resto;
                Resultado = string.Format("{0}001{1}", PIS, DV.ToString());
            }
            else
            {
                Resultado = string.Format("{0}000{1}", PIS, DV.ToString());
            }

            Result = CNS.Equals(Resultado);

            return Result;
        }

        /// <summary>
        /// Validação da Carteira Nacional de Saúde Proviória
        /// </summary>
        /// <param name="CNS"></param>
        /// <returns></returns>
        public static bool ValidaCNSProvisorio(this string CNS)
        {
            bool Result = false;

            float Soma;
            float Resto;
            string PIS = string.Empty;

            if (CNS.Trim().Length != 15)
                return Result;

            PIS = CNS.SomenteNumeros();
            if (!CNS.Equals(PIS))
                return Result;

            if ((!CNS.Substring(0, 1).Equals("7")) && (!CNS.Substring(0, 1).Equals("8")) && (!CNS.Substring(0, 1).Equals("9")))
                return Result;

            Soma = (int.Parse(PIS.Substring(00, 1)) * 15) +
                   (int.Parse(PIS.Substring(01, 1)) * 14) +
                   (int.Parse(PIS.Substring(02, 1)) * 13) +
                   (int.Parse(PIS.Substring(03, 1)) * 12) +
                   (int.Parse(PIS.Substring(04, 1)) * 11) +
                   (int.Parse(PIS.Substring(05, 1)) * 10) +
                   (int.Parse(PIS.Substring(06, 1)) * 09) +
                   (int.Parse(PIS.Substring(07, 1)) * 08) +
                   (int.Parse(PIS.Substring(08, 1)) * 07) +
                   (int.Parse(PIS.Substring(09, 1)) * 06) +
                   (int.Parse(PIS.Substring(10, 1)) * 05) +
                   (int.Parse(PIS.Substring(11, 1)) * 04) +
                   (int.Parse(PIS.Substring(12, 1)) * 03) +
                   (int.Parse(PIS.Substring(13, 1)) * 02) +
                   (int.Parse(PIS.Substring(14, 1)) * 01);

            Resto = Soma % 11;
            Result = Resto == 0;

            return Result;
        }

        /// <summary>
        /// Valida Declaração de Nascido Vivo
        /// </summary>
        /// <param name="DNV"></param>
        /// <returns></returns>
        public static bool ValidaDNV(this string DNV)
        {
            bool Result = true;

            if (Result)
                Result = DNV.Equals(SomenteNumeros(DNV));

            if (Result)
                Result = !SomenteNumeros(DNV).Equals(string.Empty);

            if (Result)
                Result = SomenteNumeros(DNV).Length <= 15;

            return Result;
        }

        /// <summary>
        /// Validando Digito verificador do Numero smiles
        /// </summary>
        /// <param name="NumeroSmile"></param>
        /// <returns></returns>
        public static bool ValidaNumeroSmile(this string NumeroSmile)
        {
            /*
             * Validação do Numero Smile
             * Ex.: 999999980 => 99999998 dividir 7 = 14285714 RESTO = 0 (último digito do Numero Smile)
             *      999999991 => 99999999 dividir 7 = 14285714 RESTO = 1
             *      999999932 => 99999993 dividir 7 = 14285713 RESTO = 2
             *      007006473 => 00700647 dividir 7 = 100092   RESTO = 3
             *      080555624 => 08055562 dividir 7 = 1150794  RESTO = 4
             *      000100015 => 00010001 dividir 7 = 1428     RESTO = 5
             *      005000166 => 00500016 dividir 7 = 71430    RESTO = 6
             */
            bool result = false;
            int intNumero = 0;
            int intDigito = -1;

            if (NumeroSmile.Trim().Length == 9)
                if (int.TryParse(NumeroSmile.Trim().Substring(0, 8), out intNumero))
                    if (intNumero > 0)
                    {
                        intDigito = intNumero % 7;
                        if ((intDigito >= 0) && (intDigito <= 6))
                            result = NumeroSmile.Equals(intNumero.ToString("00000000") + intDigito.ToString());
                    }

            return result;
        }

        public static bool ValidaRG(this string RG, string UF)
        {
            //Elimina da string os traços, pontos e virgulas,
            RG = RG.RemovePontuacao().Trim();
            UF = UF.ToUpper();

            /* Somente o último digito pode ser letra */
            for (int i = 0; i < RG.Length - 1; i++)
                if ("0123456789".IndexOf(RG[i]) < 0)
                    return false;

            if ((!UF.Equals("SP")) && (!UF.Equals("RJ")))
                return true;

            #region VALIDA SP
            if (UF.Equals("SP"))
            {
                //Verifica se o tamanho da string é 9
                if (RG.Length <= 9)
                {
                    RG = RG.PadLeft(9, '0');

                    int[] n = new int[9];

                    try
                    {
                        n[0] = Convert.ToInt32(RG.Substring(0, 1));
                        n[1] = Convert.ToInt32(RG.Substring(1, 1));
                        n[2] = Convert.ToInt32(RG.Substring(2, 1));
                        n[3] = Convert.ToInt32(RG.Substring(3, 1));
                        n[4] = Convert.ToInt32(RG.Substring(4, 1));
                        n[5] = Convert.ToInt32(RG.Substring(5, 1));
                        n[6] = Convert.ToInt32(RG.Substring(6, 1));
                        n[7] = Convert.ToInt32(RG.Substring(7, 1));
                        if (RG.Substring(8, 1).Equals("x") || RG.Substring(8, 1).Equals("X"))
                            n[8] = 10;
                        else
                            n[8] = Convert.ToInt32(RG.Substring(8, 1)) * 100;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                    //obtém cada um dos caracteres do rg

                    //Aplica a regra de validação do RG, multiplicando cada digito por valores pré-determinados
                    n[0] *= 2;
                    n[1] *= 3;
                    n[2] *= 4;
                    n[3] *= 5;
                    n[4] *= 6;
                    n[5] *= 7;
                    n[6] *= 8;
                    n[7] *= 9;
                    //n[8] *= 100;

                    //Valida o RG
                    int somaFinal = n[0] + n[1] + n[2] + n[3] + n[4] + n[5] + n[6] + n[7] + n[8];
                    if ((somaFinal % 11) == 0)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
            #endregion VALIDA SP

            #region VALIDA RJ
            if (UF.Equals("RJ"))
            {
                //Verifica se o tamanho da string é 9
                if (RG.Length <= 9)
                {
                    RG = RG.PadLeft(9, '0');

                    int[] n = new int[9];

                    try
                    {
                        n[0] = Convert.ToInt32(RG.Substring(0, 1));
                        n[1] = Convert.ToInt32(RG.Substring(1, 1));
                        n[2] = Convert.ToInt32(RG.Substring(2, 1));
                        n[3] = Convert.ToInt32(RG.Substring(3, 1));
                        n[4] = Convert.ToInt32(RG.Substring(4, 1));
                        n[5] = Convert.ToInt32(RG.Substring(5, 1));
                        n[6] = Convert.ToInt32(RG.Substring(6, 1));
                        n[7] = Convert.ToInt32(RG.Substring(7, 1));
                        n[8] = Convert.ToInt32(RG.Substring(8, 1));
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                    //obtém cada um dos caracteres do rg

                    //Aplica a regra de validação do RG, multiplicando cada digito por valores pré-determinados
                    n[0] = n[0] * 1 > 9 ? (n[0] * 1) - 9 : n[0] * 1;
                    n[1] = n[1] * 2 > 9 ? (n[1] * 2) - 9 : n[1] * 2;
                    n[2] = n[2] * 1 > 9 ? (n[2] * 1) - 9 : n[2] * 1;
                    n[3] = n[3] * 2 > 9 ? (n[3] * 2) - 9 : n[3] * 2;
                    n[4] = n[4] * 1 > 9 ? (n[4] * 1) - 9 : n[4] * 1;
                    n[5] = n[5] * 2 > 9 ? (n[5] * 2) - 9 : n[5] * 2;
                    n[6] = n[6] * 1 > 9 ? (n[6] * 1) - 9 : n[6] * 1;
                    n[7] = n[7] * 2 > 9 ? (n[7] * 2) - 9 : n[7] * 2;

                    //Valida o RG
                    int somaFinal = n[0] + n[1] + n[2] + n[3] + n[4] + n[5] + n[6] + n[7] + n[8];
                    if ((somaFinal % 10) == 0)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
            #endregion VALIDA RJ

            return false;
        }

        /// <summary>
        /// Validar Senha
        /// </summary>
        /// <param name="_Senha"></param>
        /// <param name="_Novo"></param>
        /// <param name="_QtdeCaractere"></param>
        /// <param name="_MsgErro"></param>
        /// <returns></returns>
        public static bool ValidaSenha(this string _Senha, bool _Novo, int _QtdeCaractere, out string _MsgErro)
        {
            _MsgErro = string.Empty;
            if (((_Senha.Length != _QtdeCaractere) && (!_Novo))
                || ((_Novo) && (!_Senha.Equals("123456".Substring(0, _QtdeCaractere))) && (_Senha.Length != _QtdeCaractere)))
            {
                _MsgErro = string.Format("Senha deve conter {0} caracteres!", _QtdeCaractere.ToString());
                return false;
            }

            if (!_Senha.Equals(_Senha.SomenteNumeros()))
            {
                _MsgErro = "Senha deve conter somente números!";
                return false;
            }

            if ((_Senha.Equals("123456"))
                || (_Senha.Equals("123456".Substring(0, _QtdeCaractere)))
                || (_Senha.Equals("000000".Substring(0, _QtdeCaractere)))
                || (_Senha.Equals("111111".Substring(0, _QtdeCaractere)))
                || (_Senha.Equals("222222".Substring(0, _QtdeCaractere)))
                || (_Senha.Equals("333333".Substring(0, _QtdeCaractere)))
                || (_Senha.Equals("444444".Substring(0, _QtdeCaractere)))
                || (_Senha.Equals("555555".Substring(0, _QtdeCaractere)))
                || (_Senha.Equals("666666".Substring(0, _QtdeCaractere)))
                || (_Senha.Equals("777777".Substring(0, _QtdeCaractere)))
                || (_Senha.Equals("888888".Substring(0, _QtdeCaractere)))
                || (_Senha.Equals("999999".Substring(0, _QtdeCaractere))))
            {
                if ((_Novo) && (_Senha.Equals("123456")))
                    return true;
                else
                {
                    _MsgErro = string.Format("Senha não pode ser uma sequência de números iguais ou {0}!", "123456".Substring(0, _QtdeCaractere));
                    return false;
                }
            }
            else
                return true;
        }

        /// <summary>
        /// * Regras de senha de usuário *
        ///      - Conter x caracteres numéricos passado como parametros;
        ///      - Não pode conter uma sequencia de 4 caracteres. Ex.: "01235235" (erro: 0123), "25415678" (erro: 5678), "10543285" (erro: 5432)
        ///      - Não pode conter uma sequencia de 4 caracteres iguais. Ex.: "55556324" (erro: 5555), "25746666" (erro: 6666), "74333378" (erro 3333)
        ///      - Não pode conter mais de 2 conjuto de uma sequência de 2 caracteres. Ex.: "12121212" (erro: 4 sequencias de "12" ou 3 sequencias de "21"), "57635757" (erro: 3 sequencias de "57"), "77677477" (erro: 3 sequencias de "77")
        /// </summary>
        /// <param name="senha"></param>
        /// <returns></returns>
        public static bool ValidaSenhaUsuario(this string senha, int qtdeCaracteres, out string msgErro)
        {
            bool result = true;
            msgErro = string.Empty;

            try
            {
                if (result)
                {
                    result = senha.Trim().Length == qtdeCaracteres;
                    if (!result)
                        msgErro = string.Format("A senha deve conter {0} números!", qtdeCaracteres);
                }

                if (result)
                {
                    result = senha == senha.SomenteNumeros();
                    if (!result)
                        msgErro = "A senha deve conter somente números!";
                }

                // Numeros iguais
                if (result)
                {
                    for (int i = 0; i <= 9; i++)
                        if (senha.Replace(i.ToString(), string.Empty).Length == 0)
                        {
                            result = false;
                            break;
                        }

                    // Parte iguais
                    if (result) result = senha.Replace("0000", string.Empty).Length == qtdeCaracteres;
                    if (result) result = senha.Replace("1111", string.Empty).Length == qtdeCaracteres;
                    if (result) result = senha.Replace("2222", string.Empty).Length == qtdeCaracteres;
                    if (result) result = senha.Replace("3333", string.Empty).Length == qtdeCaracteres;
                    if (result) result = senha.Replace("4444", string.Empty).Length == qtdeCaracteres;
                    if (result) result = senha.Replace("5555", string.Empty).Length == qtdeCaracteres;
                    if (result) result = senha.Replace("6666", string.Empty).Length == qtdeCaracteres;
                    if (result) result = senha.Replace("7777", string.Empty).Length == qtdeCaracteres;
                    if (result) result = senha.Replace("8888", string.Empty).Length == qtdeCaracteres;
                    if (result) result = senha.Replace("9999", string.Empty).Length == qtdeCaracteres;

                    if (!result)
                        msgErro = "A senha não pode conter um bloco de 4 dígitos iguais!";
                }

                // Sequencia
                if (result)
                {
                    if (result) result = senha.Replace("0123", string.Empty).Length == qtdeCaracteres;
                    if (result) result = senha.Replace("1234", string.Empty).Length == qtdeCaracteres;
                    if (result) result = senha.Replace("2345", string.Empty).Length == qtdeCaracteres;
                    if (result) result = senha.Replace("3456", string.Empty).Length == qtdeCaracteres;
                    if (result) result = senha.Replace("4567", string.Empty).Length == qtdeCaracteres;
                    if (result) result = senha.Replace("5678", string.Empty).Length == qtdeCaracteres;
                    if (result) result = senha.Replace("6789", string.Empty).Length == qtdeCaracteres;
                    if (result) result = senha.Replace("7890", string.Empty).Length == qtdeCaracteres;

                    if (result) result = senha.Replace("9876", string.Empty).Length == qtdeCaracteres;
                    if (result) result = senha.Replace("8765", string.Empty).Length == qtdeCaracteres;
                    if (result) result = senha.Replace("7654", string.Empty).Length == qtdeCaracteres;
                    if (result) result = senha.Replace("6543", string.Empty).Length == qtdeCaracteres;
                    if (result) result = senha.Replace("5432", string.Empty).Length == qtdeCaracteres;
                    if (result) result = senha.Replace("4321", string.Empty).Length == qtdeCaracteres;
                    if (result) result = senha.Replace("3210", string.Empty).Length == qtdeCaracteres;
                    if (result) result = senha.Replace("0987", string.Empty).Length == qtdeCaracteres;

                    if (!result)
                        msgErro = "A senha não pode conter um bloco de 4 dígitos sequenciais!";
                }

                if (result)
                {
                    for (int i = 0; i <= 9; i++)
                        for (int x = 0; x <= 9; x++)
                        {
                            string pa = string.Format("{0}{1}", i, x);
                            if (senha.Replace(pa, string.Empty).Length < qtdeCaracteres - 4)
                            {
                                result = false;
                                break;
                            }
                            if (!result)
                                break;
                        }

                    if (!result)
                        msgErro = "A senha não pode conter mais de 2 blocos com 2 dígitos sequenciais ou iguais!";
                }
            }
            catch
            {
                result = false;
                msgErro = "Erro ao validar Senha!";
            }
            return result;
        }

        public static bool ValidaHora(this string hora)
        {
            bool result = false;
            try
            {
                string[] itens = hora.Split(':');
                result = itens.Length >= 1 && itens.Length <= 3;

                int varTemp = 0;

                if (result)
                    result = itens[0].Length == 2;

                if (result)
                    result = int.TryParse(itens[0], out varTemp);

                if (result)
                    result = varTemp >= 0 && varTemp <= 23;

                if (result)
                    result = itens[1].Length == 2;

                if (result)
                    result = int.TryParse(itens[1], out varTemp);

                if (result)
                    result = varTemp >= 0 && varTemp <= 59;

                if (itens.Length == 3)
                {
                    if (result)
                        result = itens[2].Length == 2;

                    if (result)
                        result = int.TryParse(itens[2], out varTemp);

                    if (result)
                        result = varTemp >= 0 && varTemp <= 59;
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        #endregion VALIDAÇÕES

        public static string RetornaEAN13(this string CodBar)
        {
            string result = CodBar.Trim();

            // vamos retirar os carateres excedentes quando CodBar > 13, que para ser um codigo barras valido tem sempre de ser o ZERO
            if ((result.Length > 13) && (result.Substring(0, 1).Equals('0')))
                result = result.Substring(1);

            return result;
        }

        public static string RetornaDDD(this string Telefone)
        {
            string result = Telefone?.Trim() ?? "";


            result = !String.IsNullOrWhiteSpace(result) && result.Length >= 2
                    ? result.Substring(0, 2)
                    : result;

            return result;
        }

        public static string RetornaTelefone(this string Telefone)
        {
            string result = Telefone?.Trim() ?? "";

            result = !String.IsNullOrWhiteSpace(result) && result.Length > 9
                    ? result.Substring(2, result.Length - 2)
                    : result;

            return result;
        }

        public static string RetornaCep(this string Cep)
        {
            string result = Cep?.Trim() ?? "";


            result = !String.IsNullOrWhiteSpace(result) && result.Length >= 5
                    ? result.Substring(0, 5)
                    : result;

            return result;
        }

        public static string RetornaComplementoCep(this string Cep)
        {
            string result = Cep?.Trim() ?? "";

            result = !String.IsNullOrWhiteSpace(result) && result.Length > 5
                    ? result.Substring(result.Length - 3)
                    : result;

            return result;
        }

        public static bool Inside(this string text, string inside)
        {
            return inside.Contains(text);
        }

        public static string ToInsertUpdate(this string text)
        {
            if (!String.IsNullOrEmpty(text))
            {
                return "'" + text + "'";
            }
            else
            {
                return "NULL";
            }
        }

        public static string ToStringLength(this string text, int start, int Length)
        {
            if (!String.IsNullOrEmpty(text))
                return text.Substring(start, text.Length < Length ? text.Length : Length);
            else
                return text;
        }

        public static string SomenteNumeros(this string text)
        {
            string sRet = "";

            if (string.IsNullOrEmpty(text))
                sRet = text;
            else
            {
                for (int i = 0; i < text.Length; i++)
                    if ("0123456789".Contains(text[i].ToString()))
                        sRet = sRet + text[i].ToString();
            }

            return sRet;
        }

        public static string QuotedStr(this string text)
        {
            return "'" + text + "'";
        }

        
        /// <summary>
        /// Get substring of specified number of characters on the right.
        /// </summary>
        public static string Right(this string value, int length)
        {
            if (String.IsNullOrEmpty(value)) return value;
            return value.Length <= length ? value : value.Substring(value.Length - length);
        }
        public static string Left(this string value, int Length)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return (value.Length <= Length ? value : value.Substring(0, Length));
        }

        
        /// <summary>
        /// Elimina espacos, virgulas, pontos e alinha com n zeros a esquerda (nao verifica se e maior)
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>

        public static string LZeros(this string s, int Length)
        {
            return s.FastRemovePontuacao().PadLeft(Length, '0');
        }
        /// <summary>
        /// Rotina rapida para retirar espacos, virgulas e pontos
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static unsafe string FastRemovePontuacao(this string s, bool removeEmpty = true)
        {
            if (s == null) throw new ArgumentNullException("String Nula");
            int len = s.Length;
            char* newChars = stackalloc char[len];
            char* currentChar = newChars;
            if (removeEmpty)
                for (int i = 0; i < len; ++i)
                {
                    char c = s[i];
                    switch (c)
                    {
                        case ',':
                        case '.':
                        case ' ':
                        case '-':
                        case '_':
                        case '/':
                        case '\\':
                        case ';':
                        case ':':
                        case '(':
                        case ')':

                            continue;
                        default:
                            *currentChar++ = c;
                            break;
                    }
                }
            else
                for (int i = 0; i < len; ++i)
                {
                    char c = s[i];
                    switch (c)
                    {
                        case ',':
                        case '.':
                        case '-':
                        case '_':
                        case '/':
                        case '\\':
                        case ';':
                        case ':':
                        case '(':
                        case ')':
                            continue;
                        default:
                            *currentChar++ = c;
                            break;
                    }
                }
            return new string(newChars, 0, (int)(currentChar - newChars));
        }

        public static IEnumerable<string> Chunks(this string str, int chunkSize)
        {
            for (int i = 0; i < str.Length; i += chunkSize) yield return str.Substring(i, chunkSize);
        }

        public static byte[] ToHashSha256(this string value, Encoding encoding)
        {
            try
            {
                byte[] hashBytes = encoding.GetBytes(value);
                using (SHA256 mySHA256 = SHA256CryptoServiceProvider.Create())
                {
                    byte[] cryptPassword = mySHA256.ComputeHash(hashBytes);
                    return cryptPassword;
                }
            }
            catch
            {
                return null;
            }
        }

        public static String ToHashSha256(this string value, Encoding encoding, Boolean Base64)
        {
            try
            {
                if (Base64)
                    return Convert.ToBase64String(ToHashSha256(value, encoding));
                else
                    return encoding.GetString(ToHashSha256(value, encoding));
            }
            catch
            {
                return null;
            }
        }

        public static string RemoverFormatacaoZap(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;

            if (value.StartsWith("*") && value.EndsWith("*")
                || (value.StartsWith("_") && value.EndsWith("_"))
                || (value.StartsWith("~") && value.EndsWith("~"))
                )
                return value.Substring(1, value.Length - 2);

            if (value.StartsWith("```") && value.EndsWith("```"))
                return value.Substring(3, value.Length - 6);

            return value;
        }

        public static string RetornarUFAbreviado(this string uf)
        {
            return uf.ToUpper().RemovePontuacao().RemoveAcentos() switch
            {
                string AC when "ACRE" == AC => "AC",
                string AL when "ALAGOAS" == AL => "AL",
                string AP when "AMAPA" == AP => "AP",
                string AM when "AMAZONAS" == AM => "AM",
                string BA when "BAHIA" == BA => "BA",
                string CE when "CEARA" == CE => "CE",                
                string DF when "DISTRITO FEDERAL" == DF => "DF",
                string ES when "ESPIRITO SANTO" == ES => "ES",
                string GO when "GOIAS" == GO => "GO",
                string MA when "MARANHAO" == MA => "MA",
                string MT when "MATO GROSSO" == MT => "MT",
                string MS when "MATO GROSSO DO SUL" == MS => "MS",
                string MG when "MINAS GERAIS" == MG => "MG",
                string PA when "PARA" == PA => "PA",
                string PB when "PARAIBA" == PB => "PB",
                string PR when "PARANA" == PR => "PR",
                string PE when "PERNAMBUCO" == PE => "PE",
                string PI when "PIAUI" == PI => "PI",
                string RJ when "RIO DE JANEIRO" == RJ => "RJ",
                string RN when "RIO GRANDE DO NORTE" == RN => "RN",
                string RS when "RIO GRANDE DO SUL" == RS => "RS",
                string RO when "RONDONIA" == RO => "RO",
                string RR when "RORAIMA" == RR => "RR",
                string SC when "SANTA CATARINA" == SC => "SC",
                string SP when "SAO PAULO" == SP => "SP",
                string SE when "SERGIPE" == SE => "SE",
                string TO when "TOCANTINS" == TO => "TO",
                _ => $"{uf}"

            };
        }

        public static bool TryParseJson<T>(this string @this, out T result)
        {
            bool success = true;
            var settings = new JsonSerializerSettings
            {
                Error = (sender, args) => { success = false; args.ErrorContext.Handled = true; },
                MissingMemberHandling = MissingMemberHandling.Error
            };
            result = JsonConvert.DeserializeObject<T>(@this, settings);
            return success;
        }
    }
}
