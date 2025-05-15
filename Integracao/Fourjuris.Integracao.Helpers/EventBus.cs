using StackExchange.Redis;
using System.Text.Json;

namespace Fourjuris.Integracao.Helpers
{
    /// <summary>
    /// Interface para publicação e inscrição de eventos.
    /// Define métodos para publicar, inscrever e desinscrever eventos.
    /// <![CDATA[<author>Marcelo Miranda</author>]]>
    /// </summary>
    public interface IEventBus
    {
        Task PublicarAsync<T>(T evento) where T : class;
        void Inscrever<T>(Action<T> handler) where T : class;
        void Desinscrever<T>(Action<T> handler) where T : class;
    }

    /// <summary>
    /// Implementação de um barramento de eventos usando Redis pub/sub.
    /// O barramento de eventos é utilizado para publicar e inscrever eventos entre processos via Redis.
    /// Usa o padrão pub/sub do Redis para comunicação em tempo real.
    /// <![CDATA[<author>Marcelo Miranda</author>]]>
    /// </summary>
    public class RedisEventBus : IEventBus, IDisposable
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly ISubscriber _subscriber;

        /// <summary>
        /// Construtor do RedisEventBus.
        /// Inicializa a conexão com o Redis usando a string de conexão fornecida.
        /// <![CDATA[<author>Marcelo Miranda</author>]]>
        /// </summary>
        /// <param name="connectionString">String de conexão do Redis (ex.: "localhost:6379").</param>
        public RedisEventBus(string connectionString = "localhost:6379")
        {
            _redis = ConnectionMultiplexer.Connect(connectionString);
            _subscriber = _redis.GetSubscriber();
        }

        /// <summary>
        /// Publica um evento no canal Redis correspondente ao tipo do evento.
        /// Serializa o evento para JSON e publica no canal com o nome do tipo do evento.
        /// <![CDATA[<author>Marcelo Miranda</author>]]>
        /// </summary>
        /// <typeparam name="T">Tipo do evento a ser publicado.</typeparam>
        /// <param name="evento">Instância do evento a ser publicado.</param>
        /// <returns>Task representando a operação assíncrona.</returns>
        public async Task PublicarAsync<T>(T evento) where T : class
        {
            if (evento == null)
                throw new ArgumentNullException(nameof(evento));

            var channel = typeof(T).Name;
            var message = JsonSerializer.Serialize(evento);
            await _subscriber.PublishAsync(channel, message);
        }

        /// <summary>
        /// Inscreve um manipulador para eventos do tipo especificado.
        /// O manipulador será chamado sempre que um evento do tipo T for publicado no canal Redis.
        /// <![CDATA[<author>Marcelo Miranda</author>]]>
        /// </summary>
        /// <typeparam name="T">Tipo do evento a ser inscrito.</typeparam>
        /// <param name="handler">Ação a ser executada quando o evento for recebido.</param>
        public void Inscrever<T>(Action<T> handler) where T : class
        {
            var channel = typeof(T).Name;
            _subscriber.Subscribe(channel, (ch, message) =>
            {
                var evento = JsonSerializer.Deserialize<T>(message);
                handler(evento);
            });
        }

        /// <summary>
        /// Remove a inscrição de um manipulador para eventos do tipo especificado.
        /// Para de escutar eventos do tipo T no canal Redis.
        /// <![CDATA[<author>Marcelo Miranda</author>]]>
        /// </summary>
        /// <typeparam name="T">Tipo do evento a ser desinscrito.</typeparam>
        /// <param name="handler">Ação que foi inscrita anteriormente.</param>
        public void Desinscrever<T>(Action<T> handler) where T : class
        {
            var channel = typeof(T).Name;
            _subscriber.Unsubscribe(channel);
        }

        /// <summary>
        /// Libera os recursos do RedisEventBus.
        /// Fecha a conexão com o Redis.
        /// <![CDATA[<author>Marcelo Miranda</author>]]>
        /// </summary>
        public void Dispose()
        {
            _redis?.Dispose();
        }
    }
}