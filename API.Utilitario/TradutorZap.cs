using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

namespace AppPromotora.Api.Utilitario
{
    public static class TradutorZap
    {

        public static string GeraContainerPergunta(string alternativas, string nomeContainer, string alias, string frase, string tipoPergunta = "ALTERNATIVE", string respostaCerta = "N", string tipoResposta = "N")
        {
            var pergunta = JsonSerializer.Serialize(new List<Object>
            {

                new
                {
                    PropName = "Type",
                    PropValue = tipoPergunta
                },
                new {
                    PropName = "Alias",
                    PropValue = alias
                },
                new
                {
                    PropName = "Phrase",
                    PropValue = frase
                },
                new
                {
                    PropName = "RightAnswer",
                    PropValue  = respostaCerta
                },
                new
                {
                    PropName = "Enumeration",
                    PropValue  = tipoResposta
                },
                new {
                    PropName = "Active",
                    PropValue = "S"
                },
                new
                {
                    PropName = "Alternatives",
                    PropValue  = "$"

                }


            });

            pergunta = pergunta.Replace("\"", "\\\"").Replace("$", alternativas);

            var container = JsonSerializer.Serialize(new List<Object>
            {
                new
                {
                    PropName = nomeContainer,
                    PropValue = "$"
                }
            });


            container = container.Replace("$", pergunta);
            return container;
        }

        public static string GeraAlternativa(IEnumerable<Tuple<string, string>> value)
        {
            int inicio = 1;

            var alternativas = JsonSerializer.Serialize((
                            from a in value
                            select new
                            {
                                PropName = $"Alternative{inicio:000}",
                                PropValue = JsonSerializer.Serialize(new List<Object> {
                                    new {
                                        PropName = "Alias",
                                        PropValue = a.Item1
                                    },
                                    new {
                                        PropName = "Phrase",
                                        PropValue = a.Item2
                                    },
                                    new {
                                        PropName = "Order",
                                        PropValue = $"{inicio++}"
                                    },
                                    new {
                                        PropName = "Active",
                                        PropValue = "S"
                                    }
                                }).Replace("u0022", "\\\\\\\\\\\\\\\"")

                            }
                        ).ToList()).Replace("u0022", "\\\\\\\"").Replace("\"", "\"");

            alternativas = alternativas.Replace("\"", "\\\\\\\"");
            return alternativas;
        }

        public static string GeraMensagem(string nomePropriedade,string frase, string valor)
        {

            var json = JsonSerializer.Serialize(new List<Object>
            {
                new
                {
                    PropName = nomePropriedade,
                    PropValue = new List<Object>
                    {
                        new
                        {
                            PropName = "Type",
                            PropValue = "MESSAGE"
                        },
                        new
                        {
                            PropName = "Phrase",
                            PropValue = $"{frase}{valor}"
                        }
                    }
                }
            });

            return json;
        }

        public static string GeraParametros(string nomePropriedade, List<Tuple<string, string>> expressoes)
        {

            var json = JsonSerializer.Serialize(new List<Object>
            {
                new
                {
                    PropName = nomePropriedade,
                    PropValue = new List<Object>
                    {
                        new
                        {
                            PropName = "Type",
                            PropValue = "EXECFUNCTION"
                        },
                        new
                        {
                            PropName = "Expression",
                            PropValue = string.Join("", expressoes.Select(item => $"addparm({item.Item1},{item.Item2})") )
                        }
                    }
                }
            });

            return json;
        }

        public static string RetornoTripleDes(string EncKey, string DecKey, string iVRecebido, string json)
        {

            TripleDESCryptoServiceProvider auxTdes = new TripleDESCryptoServiceProvider();
            auxTdes.GenerateIV();
            byte[] IVNovoArray = auxTdes.IV;
            string IVNovo = Convert.ToBase64String(IVNovoArray);
            //Encriptação do JSON de resposta ao AnnA

            string AnnAresponse = Cript3Des.Encrypt(json, EncKey, IVNovo);


            //Encriptação do "IV Novo" com a chave de desencriptação e o "IV Recebido"
            string IVNovoEncriptado = Cript3Des.Encrypt(IVNovo, DecKey, iVRecebido);


            //Concatenação do JSON de resposta encriptado, o "IV Recebido" e o "IV Novo Encriptado"
            AnnAresponse += iVRecebido + IVNovoEncriptado;

            return AnnAresponse;
        }
    }
}
