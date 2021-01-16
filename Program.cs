using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brickwork
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome builder! \nYou must cover an area of size M x N. " +
                "\nEnter even numbers between 1 and 100 for width-N and length-M." +" \nEnter N for rows:");
            int rowSize;
            do
            {
                Console.WriteLine("Enter an even number (from 1 to 100) for N:");
                try
                {
                    rowSize = int.Parse(Console.ReadLine());
                } catch (FormatException)
                {
                    // The entered symbol is not a number 
                    rowSize = -1;
                }
            } while (!IsEven(rowSize) || rowSize <= 0 || rowSize > 100);

            Console.WriteLine("Enter M for columns:");
            int columnSize;
            do
            {
                Console.WriteLine("Enter an even number (from 1 to 100) for M:"); 
                try
                {
                    columnSize = int.Parse(Console.ReadLine());
                }
                catch (FormatException)
                {
                    // The entered symbol is not a number 
                    columnSize = -1;
                }
            } while (!IsEven(columnSize) || columnSize <= 0 || columnSize > 100);

            Console.WriteLine("Your brickwall must be with size " + rowSize + "x" + columnSize + ".");  
            Console.WriteLine("Please enter " + rowSize + " rows with " + columnSize + 
                                ".\nEnter your first layer:");
            int i = 0;
            int j = 0;
            int[,] inputArr = new int[rowSize, columnSize];
            int[] numbers;
            //Reading the numbers for each row as line from console
            do
            {
                string line = Console.ReadLine();
                try
                {
                    numbers = line.Split(new Char[] { ' ' },
                            StringSplitOptions.RemoveEmptyEntries).Select(item => int.Parse(item)).ToArray();
                    if (numbers.Length == columnSize)
                    {
                        for (j = 0; j < columnSize; j++)
                            inputArr[i, j] = numbers[j];
                        i++;
                    }
                    else
                    {
                        Console.WriteLine("Incorrect count of numbers on row " + (i + 1) + 
                                    "\nEnter " + columnSize + " numbers");
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("Enter only " + columnSize + " numbers");
                }
            } while (i < rowSize);

            // If there is no error after checking for brick spanning 3 rows or columns
            //If there is no error print in the console Layer 1 and format Layer 2
            if (!IsBrickSpanningTreeRowsOrColumns(rowSize, columnSize, inputArr))
            {
                //Write Layer 1 
                Console.WriteLine("\nLayer 1:");
                for (i = 0; i < rowSize; i++)
                {
                    for (j = 0; j < columnSize; j++)
                    {
                        Console.Write(inputArr[i, j] + " ");
                    }
                    Console.WriteLine();
                }

                Console.WriteLine();
                Console.WriteLine("Layer 2:");

                //Create two-dimensional array with the numbers for the second layer
                int[,] outputArr = new int[rowSize, columnSize];
                int currentNumber = 1;

                for (i = 0; i < rowSize - 1; i += 2)
                {
                    j = 0;
                    do
                    {
                        //Check if the column position is last? If it is the last position set the brick on the vertical
                        if (j == columnSize - 1)
                        {
                            outputArr[i, j] = currentNumber;
                            outputArr[i + 1, j] = currentNumber;
                            currentNumber++;
                            j++;
                        }
                        //Check if on the row there are two neighbouring numbers on right side. 
                        //If they are the same, set the brick on the vertical 
                        else if (inputArr[i, j] == inputArr[i, j + 1])
                        {
                            outputArr[i, j] = currentNumber;
                            outputArr[i + 1, j] = currentNumber;
                            currentNumber++;
                            j++;
                        }
                        //In the other case two horizontal bricks are being set one under the other
                        else
                        {
                            outputArr[i, j] = currentNumber;
                            outputArr[i, j + 1] = currentNumber;
                            currentNumber++;
                            outputArr[i + 1, j] = currentNumber;
                            outputArr[i + 1, j + 1] = currentNumber;
                            currentNumber++;
                            j += 2;
                        }
                    } while (j < columnSize);
                }

                /*
                    Check if layer 2 will need a two-digit number. 
                    It is because the numbers on the bricks on layer 2 are
                    numbers which are in ascending order and starting from 1.
                */
                bool isTwoDigitsNumber = (columnSize * rowSize) / 2 > 9;

                // Printing in the console the bricks surrounded by dashs                 
                WriteDashRow(isTwoDigitsNumber, columnSize);
                Console.WriteLine();
                for (i = 0; i < rowSize; i++)
                {
                    string rowDelimiter = "|";
                    for (j = 0; j < columnSize; j++)
                    {
                        // If you want to print the asterisk symbol instead of " " between the numbers in the brick use this line
                        // Console.Write(j > 0 && outputArr[i, j - 1] == outputArr[i, j] ? "*" : "|"); 
                        // instead of the next
                        Console.Write(j > 0 && outputArr[i, j - 1] == outputArr[i, j] ? " " : "|");
                        Console.Write(outputArr[i, j] + (isTwoDigitsNumber && outputArr[i, j] < 10 ? " " : ""));
                        if (i < rowSize - 1 && outputArr[i, j] == outputArr[i + 1, j])
                        { // If you want to print the asterisk symbol instead of " " between the numbers in the brick use this line
                          //  rowDelimiter += isTwoDigitsNumber ? "**|" : "*|";
                          //// instead of the next
                            rowDelimiter += isTwoDigitsNumber ? "  |" : " |";
                        }
                        else
                        {
                            rowDelimiter += isTwoDigitsNumber ? "--" : "-";
                            rowDelimiter += j < columnSize - 1 && outputArr[i, j] == outputArr[i, j + 1] ? "-" : "|";
                        }
                    }

                    Console.WriteLine("|");
                    if (i < rowSize - 1)
                    {
                        Console.WriteLine(rowDelimiter);
                    }
                }
                WriteDashRow(isTwoDigitsNumber, columnSize);
                Console.WriteLine();

            }
            else
            {
                Console.WriteLine("-1 Error: No solution exists");
            }
            Console.WriteLine("Press Enter to close...");
            Console.ReadLine();
        }

        //Check if there are bricks spanning 3 rows/ columns    
        private static bool IsBrickSpanningTreeRowsOrColumns(int N, int M, int[,] inputArr)
        {
            bool hasError = false;
            int i = 0;
            int j = 0;
            if (M > 2)
            { 
                do
                {
                    do
                    {
                        if (inputArr[i, j] == inputArr[i, j + 1] &&
                                        inputArr[i, j + 1] == inputArr[i, j + 2])
                        {
                            Console.WriteLine("Error: The input consists of the same number in three neighboring columns.");
                            hasError = true;
                        }
                        else
                            j++;
                    } while (j < M - 2 && !hasError);
                    i++;
                    j = 0;
                
                } while (i < N - 1 && !hasError) ;
            }
            if (N > 2 && !hasError)
            {
                i = 0;
                j = 0;
                do
                {
                    do
                    {
                        if (inputArr[i, j] == inputArr[i + 1, j] &&
                                inputArr[i + 1, j] == inputArr[i + 2, j])
                        {
                            Console.WriteLine("Error: The input consists of the same number in three neighboring row.");
                            hasError = true;
                        }
                        else
                            i++;
                    } while (i < N - 2 && !hasError);
                    j++;
                    i = 0;
                } while (j < M - 1 && !hasError);
            }
            return hasError;
        }

        //Write a dash row
        private static void WriteDashRow(bool number, int M)
        {
            for (int j = 0; j < (number ? 3 : 2) * M + 1; j++)
            {
                Console.Write("-");
            }
        }

        //Check is number even
        private static bool IsEven(int number)
        {
            return number % 2 == 0;
        }
    }
}
