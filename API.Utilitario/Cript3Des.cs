using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AppPromotora.Api.Utilitario
{
    public static class Cript3Des
    {

        

        private static void ExemploChamada()
        {

            //Chave de desencriptação do websevice cadastrado no AnnA
            string DecKey = "9Ae/TxLPGAEIARdg/kxjzzyNtuWJshzL"; //Deve ser alterada conforme o cadastro do webservice em sua empresa 
                                                                //Chave de encriptação do websevice cadastrado no AnnA
            string EncKey = "0R26bzX8G1bthQx6Ds2tP/TILMhh/Udi"; //Deve ser alterada conforme o cadastro do webservice em sua empresa 
                                                                //Ler a variável de POST "ANNAEXEC" que contém o "IV Recebido" sem encriptação
            string IVRecebido = "ANNAEXEC";
            //Todas as demais varáveis recebidas no POST, com exceção da variável ANNAEXEC que contém o "IV Recebido" serão enviadas com seu conteúdo encriptado
            //Leitura da variável 'AnotherVar' enviada no POST encriptada
            string MyVar1 = "anotherVar";
            MyVar1 = Decrypt(MyVar1, DecKey, IVRecebido);
            // ... Código do webservice.
            //Neste exemplo retornaremos um container tipo MSG com o valor desencriptado da variável enviada no POST 'AnotherVar' 
            string json = "{";
            json += "  \"ClientStatus\":\"OK\",";
            json += "  \"ClientMessage\":\"\",";
            json += "  \"DevCallback\":\"\",";
            json += "  \"Containers\":[";
            json += "      {";
            json += "      \"Type\":\"MSG\",";
            json += "  	\"Phrase\":\"O valor de AnotherVar é: " + MyVar1 + "\",";
            json += "      \"Alias\":\"AliasAnotherVar\",";
            json += "      \"Subject\":\"\",";
            json += "  	\"Topic\":\"\",";
            json += "  	\"Scope\":\"\",";
            json += "      \"AnswerType\":\"\",";
            json += "      \"AnswerTypeComment\":\"\",";
            json += "      \"MediaURL\":\"\",";
            json += "      \"ShowMsgHeader\":\"\",";
            json += "  	\"WaitNext\":0,";
            json += "      \"Enumeration\":\"\",";
            json += "      \"JumpType\":\"\",";
            json += "      \"JumpTo\":\"\",";
            json += "      \"ResumeType\":\"\",";
            json += "      \"ResumeTo\":\"\",";
            json += "      \"WsEncodeUrl\":\"\",";
            json += "  	\"WsUrl\":\"\",";
            json += "      \"WsCallBackUrl\":\"\",";
            json += "      \"WsCallBackMsg\":\"\",";
            json += "      \"IgnoreServices\":\"\",";
            json += "      \"ExternalData\":\"\",";
            json += "      \"RespostaDefault\":\"\",";
            json += "      \"GroupAlias\":\"\",";
            json += "      \"IsSensitive\":\"\"";
            json += "  	}";
            json += "  ]";
            json += "}";
            // ... Fim do código do webservice.
            //Criar novo IV
            TripleDESCryptoServiceProvider auxTdes = new TripleDESCryptoServiceProvider();
            auxTdes.GenerateIV();
            byte[] IVNovoArray = auxTdes.IV;
            string IVNovo = Convert.ToBase64String(IVNovoArray);
            //Encriptação do JSON de resposta ao AnnA
            string AnnAresponse = Encrypt(json, EncKey, IVNovo);
            //Encriptação do "IV Novo" com a chave de desencriptação e o "IV Recebido"
            string IVNovoEncriptado = Encrypt(IVNovo, DecKey, IVRecebido);
            //Concatenação do JSON de resposta encriptado, o "IV Recebido" e o "IV Novo Encriptado"
            AnnAresponse += IVRecebido + IVNovoEncriptado;
           

        }

        // Funções de encrypt e decrypt
        public static string Encrypt(string data, string key, string IV)
        {
            //Converter od dados a serem encriptados de base64 string para um array de bytes
            byte[] dataArray = UTF8Encoding.UTF8.GetBytes(data);

            //Converter a chave de encriptação de base64 string para um array de bytes
            byte[] keyArray = Convert.FromBase64String(key);

            //Converter o IV de base64 string para um array de bytes
            byte[] IVArray = Convert.FromBase64String(IV);
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.IV = IVArray;
            tdes.Mode = CipherMode.CBC; //Cipher Block Chaining
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(dataArray, 0, dataArray.Length);
            tdes.Clear();

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        public static string Decrypt(string data, string key, string IV)
        {
            //Converter os dados encriptados de base64 string para um array de bytes
            byte[] dataArray = Convert.FromBase64String(data);
            //Converter a chave de desencriptação de base64 string para um array de bytes
            byte[] keyArray = Convert.FromBase64String(key);
            //Converter o IV de base64 string para um array de bytes
            byte[] IVArray = Convert.FromBase64String(IV);
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.IV = IVArray;
            tdes.Mode = CipherMode.CBC; //Cipher Block Chaining
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(dataArray, 0, dataArray.Length);
            tdes.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        /*
        public static string Encrypt(string toEncrypt, string key, string IV, bool useHashing = false)
        {
            byte[] keyArray;
            byte[] toEncryptArray = Convert.FromBase64String(toEncrypt);
            byte[] IVArray = Convert.FromBase64String(IV);

            //System.Windows.Forms.MessageBox.Show(key);
            //If hashing use get hashcode regards to your key
            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                //Always release the resources and flush data
                // of the Cryptographic service provide. Best Practice

                hashmd5.Clear();
            }
            else
                keyArray = Convert.FromBase64String(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes.
            //We choose ECB(Electronic code Book)
            tdes.Mode = CipherMode.CBC;
            //padding mode(if any extra byte added)            
            tdes.Padding = PaddingMode.PKCS7;

            tdes.IV = IVArray;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            //transform the specified region of bytes array to resultArray
            byte[] resultArray =
              cTransform.TransformFinalBlock(toEncryptArray, 0,
              toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor
            tdes.Clear();
            //Return the encrypted data into unreadable string format
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string Decrypt(string cipherString, string key, string IV, bool useHashing= false)
        {
            byte[] keyArray;
            //get the byte code of the string

            byte[] toEncryptArray = Convert.FromBase64String(cipherString);
            byte[] IVArray = Convert.FromBase64String(IV);

            if (useHashing)
            {
                //if hashing was used get the hash code with regards to your key
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                //release any resource held by the MD5CryptoServiceProvider

                hashmd5.Clear();
            }
            else
            {
                //if hashing was not implemented get the byte code of the key
                keyArray = Convert.FromBase64String(key);
            }

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes. 
            //We choose ECB(Electronic code Book)

            tdes.Mode = CipherMode.CBC;
            //padding mode(if any extra byte added)
            tdes.Padding = PaddingMode.PKCS7;

            tdes.IV = IVArray;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(
                                 toEncryptArray, 0, toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor                
            tdes.Clear();
            //return the Clear decrypted TEXT
            return UTF8Encoding.UTF8.GetString(resultArray);
        }*/
    }
}
