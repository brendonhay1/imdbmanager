# imdbmanager
Final project for BU C# course

IMDb manager is a simple application that utilizes data complied from the Internet Movie Database's top 250 movies. The data from IMDb, which includes elements such as movie title, ranking, director, MPAA rating, and runtime, has been complied into a CSV file. My program uses this data to construct objects of a class I call Movies. 

I use the Movies class to retrieve the title and ranking of every movie on the list, which I allow the user to print to the console. The user can also search for a spcific title within the top 250. If the film is in the list, the program will format and print to the console the data it retrieved from the Movies object. The user then has the option to add this film to a watch list. If they choose to add it to their list, the program takes the title property from the Movies object and adds it to their watch list. The user can also add a new film manually. This data is saved to a text file and persists after the user exits the program. Finally, the user can delete items from the watch list if they so choose. 

Data source:
https://www.kaggle.com/bartius/imdb-top-250-movies-info

Note: As it is used in my project, the 'cast' field is removed the CSV file. 
