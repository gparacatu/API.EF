using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AppPromotora.Api.Utilitario
{
    public static class Util
    {
        public static int ObterIdade(DateTime dataDeNascimento)
        {
            try
            {                
                int idade = DataEHora.PegaHoraBrasilia().Year - dataDeNascimento.Year;

                if (DataEHora.PegaHoraBrasilia().Month < dataDeNascimento.Month || (DataEHora.PegaHoraBrasilia().Month == dataDeNascimento.Month && DataEHora.PegaHoraBrasilia().Day < dataDeNascimento.Day))
                {
                    idade--;
                }

                return (idade < 0) ? 0 : idade;
            }
            catch
            {
                return 0;
            }
        }

        public static bool VerificarPropriedadesNaoNulas<T>(this T obj)
        {
            return typeof(T).GetProperties().All(a => a.GetValue(obj) != null);
        }

        public static bool VerificarSeExistePropriedades<T>(this T obj)
        {
            var objeto = typeof(T).GetProperties().Count(a => a.GetValue(obj) != null);
            if (objeto > 0)
                return true;
            return false;
        }

        public static List<ValidationResult> getValidationErros(object obj)
        {
            var resultadoValidacao = new List<ValidationResult>();
            var contexto = new ValidationContext(obj, null, null);
            Validator.TryValidateObject(obj, contexto, resultadoValidacao, true);
            return resultadoValidacao;
        }
    }
}
