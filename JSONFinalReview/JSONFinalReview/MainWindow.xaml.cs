/*Annie Decker
* The questions your application need to answer for the user, are as follows:
* List all of the different genres for the movies
* Which movie has the highest IMDB score?
*List all of the different movies that have a number of voted users with 350000 or more
*How many movies where Anthony Russo is the director?
*How many movies where Robert Downey Jr. is the actor 1?
 */
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JSONFinalReview
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ClearAll();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            //because more than 1 movie being stored we have to create a list
            List<Movie> MoviesNumberUsersVoted = new List<Movie>();


            //gets all of the data for the movies
            List<Movie> movies = GettingDataFromWebservice();

            //List all of the different genres for the movies
            GetAllGenres(movies);

            //Which movie has the highest IMDB score?
            GetHighestScoringMovies(movies);

            //List all of the different movies that have a number of voted users with 350000 or more
            GetMoviesWithVotesGreaterThan(movies, 350000);

            //How many movies where Anthony Russo is the director?
            AnthonyRussoDirectedMovies(movies);

            //How many movies where Robert Downey Jr. is the actor 1 ?
            RDJMovies(movies);

        }

        private void RDJMovies(List<Movie> movies)
        {
            int count = 0;
            foreach (var movie in movies)
            {
                if (movie.actor_1_name == "Robert Downey Jr.")
                {
                    count++;
                }
            }
            txtIronMan.Text = count.ToString("N0");
        }

        private void AnthonyRussoDirectedMovies(List<Movie> movies)
        {
            int count = 0;
            foreach (var movie in movies)
            {
                if (movie.director_name == "Anthony Russo")
                {
                    count++;
                }
            }
            txtAnthony.Text = count.ToString("N0");
            
        }

        /// <summary>
        /// getting the data from the webservice
        /// </summary>
        /// <returns>a list of movies</returns>
        private static List<Movie> GettingDataFromWebservice()
        {
            List<Movie> movies;
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(@"http://pcbstuou.w27.wh-2.com/webservices/3033/api/Movies?number=100").Result;
                var content = response.Content.ReadAsStringAsync().Result;
                movies = JsonConvert.DeserializeObject<List<Movie>>(content);
            }

            return movies;
        }

        /// <summary>
        /// Get all movies with with 350000 or more votes
        /// </summary>
        /// <param name="movies">The movie data to go through</param>
        /// <param name="v">value of votes movie should be greater than or equal to</param>
        private void GetMoviesWithVotesGreaterThan(List<Movie> movies, int v)
        {
            foreach (var movie in movies)
            {
                if (movie.num_user_for_reviews >= v)
                {
                    lstVote.Items.Add(movie);
                }
            }
        }

        /// <summary>
        /// Get the highest scoing movie(s)
        /// </summary>
        /// <param name="movies"></param>
        /// <param name="highestIMDBscores"></param>
        private void GetHighestScoringMovies(List<Movie> movies)
        {
            List<Movie> highestIMDBscores = new List<Movie>();
            foreach (var movie in movies)
            {
                if (highestIMDBscores.Count < 1)
                {
                    highestIMDBscores.Add(movie);
                }
                else
                {
                    if (highestIMDBscores[0].imdb_score < movie.imdb_score) //new highest scoring movie
                    {
                        highestIMDBscores.Clear();
                        highestIMDBscores.Add(movie);
                    }
                    else if (highestIMDBscores[0].imdb_score == movie.imdb_score)
                    {
                        highestIMDBscores.Add(movie);
                    }
                    else
                    {
                        //don't have to do anything
                    }
                }
            }
            if (highestIMDBscores.Count() > 1)
            {
                string content = "";
                foreach (var m in highestIMDBscores)
                {
                    content += m.movie_title + "\n";
                }
                txtScore.Text = content;
            }
            else
            {
                Hyperlink h = new Hyperlink();
                h.NavigateUri = new Uri(highestIMDBscores[0].movie_imdb_link);
                txtScore.Inlines.Add(highestIMDBscores[0].movie_title);
                h.RequestNavigate += LinkOnRequestNavigate;
                txtScore.Inlines.Add(h);
            }
            
        }

        private void LinkOnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }

        /// <summary>
        /// getting the genres
        /// </summary>
        /// <param name="movies">pulls genres from the list of movies</param>
        private void GetAllGenres(List<Movie> movies)
        {
            foreach (var movie in movies)
            {
                if (movie.genres.Contains("|"))
                {
                    var gs = movie.genres.Split('|');
                    foreach (var g in gs)
                    {
                        lstGenres.Items.Add(g);
                    }
                }
                else
                {
                    lstGenres.Items.Add(movie.genres);
                }
                
            }
        }
        /// <summary>
        /// clears all output controls
        /// </summary>
        private void ClearAll()
        {
            txtAnthony.Inlines.Clear();
            txtIronMan.Inlines.Clear();
            txtScore.Inlines.Clear();
            lstGenres.Items.Clear();
            lstVote.Items.Clear();
        }
    }
}
