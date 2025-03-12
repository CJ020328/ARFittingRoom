import java.util.ArrayList;
import java.util.Scanner;
public class LibrarySystem{
	public static void main(String[] args) {		
		//This code is to add books to the library and sort them
		ArrayList<String> LB = new ArrayList<String>(); // an array list of library books
		String nbt; //A variable to add a new book title
		String elementToSearch = null;
		Boolean Found;
		System.out.println("Create the book list by entering the book titles book by book");		
		System.out.println("Enter a book title to be added to the library");
		Scanner input = new Scanner(System.in);		
		nbt = input.next();		
		while (!(nbt.equals("end"))) {		
			LB.add(nbt);
			System.out.print("Enter a book title to be added to the library");
			nbt = input.next();			
			}
		System.out.println("The books in the library before sorting are: ");
		   for(String counter: LB){
				System.out.println(counter);
			}
		 //bubble sort--------------------------------------------------
		   boolean sorted = false;
		    String temp = null;
		    while(!sorted) {
		        sorted = true;
		        for (int i = 0; i < LB.size() - 1; i++) {		           
		        	if((LB.get(i)).compareToIgnoreCase(LB.get(i+1))>0) {
		                temp = LB.get(i);
		                LB.set(i,LB.get(i+1));
		                LB.set(i+1,temp);
		                sorted = false;
		            }
		        }
		    }
		 //-------------------------------------------------------
		 System.out.println("The books in the library after sorting are: ");
		   for(String counter: LB){
				System.out.println(counter);
			}
		  
		 //This code is to add members to the library and sort them
			ArrayList<String> LM = new ArrayList<String>();// an array list of library members
			String nMn;//A variable to add a new member name			
			System.out.println("Create the members list by entering the member names one by one");		
			System.out.println("Enter a member name to be added to the library system");
			input = new Scanner(System.in);		
			nMn = input.next();		
			while (!(nMn.equals("end"))) {		
				LM.add(nMn);
				System.out.print("Enter a memer name to be added to the library");
				nMn = input.next();			
				}
			System.out.println("The members in the library before sorting are: ");
			   for(String counter: LM){
					System.out.println(counter);
				}
			 //bubble sort--------------------------------------------------
			   sorted = false;
			   temp = null;
			    while(!sorted) {
			        sorted = true;
			        for (int i = 0; i < LM.size() - 1; i++) {		           
			        	if((LM.get(i)).compareToIgnoreCase(LM.get(i+1))>0) {
			                temp = LM.get(i);
			                LM.set(i,LM.get(i+1));
			                LM.set(i+1,temp);
			                sorted = false;
			            }
			        }
			    }
			 //-------------------------------------------------------
			 System.out.println("The members in the library after sorting are: ");
			   for(String counter: LM){
					System.out.println(counter);
				}		   
			  			   
			 //This code is to search for a specific book
		     System.out.println("Enter a book title to search for:");
			   elementToSearch = input.next();	
			   Found = false;	
			   
		   //Linear Search----------------------------------------------
		   for (int index = 0; index < LB.size(); index++) {
		        if (LB.get(index).equals(elementToSearch)) 
		        	Found = true;     
		    }
		   //------------------------------------------------------------
		   if (Found == true)
		       System.out.println("The book is found");
		   else if (Found == false)
				   System.out.println("The book is not found");
		   //--------------------------------------------------------------
		   
		 //This code is to search for a specific member
		     System.out.println("Enter a member name to search for:");
			   elementToSearch = input.next();	
			   Found = false;	
			   
		   //Linear Search----------------------------------------------
		   for (int index = 0; index < LM.size(); index++) {
		        if (LM.get(index).equals(elementToSearch)) 
		        	Found = true;     
		    }
		   //------------------------------------------------------------
		   if (Found == true)
		       System.out.println("The member is found");
		   else if (Found == false)
				   System.out.println("The member is not found");
		   //--------------------------------------------------------------
	}
}