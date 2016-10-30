using System.ComponentModel.DataAnnotations;

namespace ChatApp.Data.Surrogates
{
    public class BaseSurrogate<T> 
    {
        [Key]
        public T ID { get; set; }
    }
}
