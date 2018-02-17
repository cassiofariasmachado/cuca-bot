namespace CucaBot.Utils
{
    public class CucaBotConfig
    {
        public readonly static string[] Greetings = new string[] { "Olá", "Oi", "E ae", "Fala ae" };

        public readonly static string[] Bye = new string[] { "Até mais", "Até logo", "Tchau", "Adeus" };

        public const string Tasks = "* Agendar um novo \"dia da cuca\" \n" +
                                        "* Listar as próximas cucas \n" +
                                        "* Salvar quem irá participar \n" +
                                        "* Identificar se uma imagem é de uma cuca ou não \n" +
                                        "* Tirar algumas dúvidas sobre o cucas e sobre o \"dia da cuca\" \n";
    }
}