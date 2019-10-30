using System.Collections.Generic;
using System.Threading.Tasks;

public abstract class Client {

    public abstract Task<List<Review>> getReviews(string research);

}