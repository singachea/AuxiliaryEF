Auxiliary Entity Framework Library to Handle Self-Referenced Entity for Cascade Delete
======================================================================================

This is one of the possible ways to handle the case in SQL Server in which you can't delete with cascade for self-referenced entity. To clearly visualize the case, let's see proposed class:


	public class Person
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public virtual Person Parent { get; set; }
		public virtual List<Person> Children { get; set; }
	}

and 

	public class DbRepo : DbContext
	{
		public DbSet<Person> People { get; set; }
	}


This class is simple, but you will have problem if you try to delete something like this:

	using (var context = new DbRepo())
	{
		var p = context.People.First();
		context.People.Remove(p);
		context.SaveChanges();
	}

Of course, you can include context.People.Include("Children"), but it only sets the direct children have null parent id and it doesn't delete on cascade. Also you can't set the database either because it only allows "NO ACTION" for self-referenced table.

A simple solution would be to get all descendants of a record and delete them. This is a common scenario that we might face for almost every project, and it would be very troublesome if we have to write function for iterating this hierarchy.

Well, for this case, I decided to write a simple extension method for DbSet so that we only need to plug and play.


How to use this small library
=============================

1. Add Reference for AuxiliaryEF.dll into your project (I used EF 4.3)
2. Use "using" tag to link the extension method in your file:

	using AuxiliaryEF;

3. Then you are ready to go with this example: 
	
	using (var context = new DbRepo())
	{
		var p = context.People.First();
		context.People.DeleteAllChildrenInSQLServerSelfReference(p, e => e.Children);
		context.SaveChanges();
	}


I wrote this for the sake of laziness while I'm eating noodle; thus mistake does happen and improvements need to be heard. If you feel you want to add-on/comment, don't hesitate to contact me at singachea{at}gmail.com
