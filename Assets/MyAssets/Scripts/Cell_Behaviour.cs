using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Cell_Behaviour : Cell
{

    /*    Any live cell with fewer than two live neighbours dies, as if by underpopulation.
    Any live cell with two or three live neighbours lives on to the next generation.
    Any live cell with more than three live neighbours dies, as if by overpopulation.
    Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.

These rules, which compare the behavior of the automaton to real life, can be condensed into the following:

    Any live cell with two or three live neighbours survives.
    Any dead cell with three live neighbours becomes a live cell.
    All other live cells die in the next generation. Similarly, all other dead cells stay dead.*/
    [TableMatrix]
    [SerializeField]
    int[,] neighborMatrix = new int[3, 3];

    //We store the neighbors in a 3x3 boolean matrix, for each "alive" neighbor we count +1

    [Button]
    void CheckNeighbors()
    {
        //At the moment this matrix is filled with test data. However later-on we will have to base these values on the neighbor cell's alive state where true = 1 and false = 0.
        //Note that the middle value of the whole matrix represents our cell. We should take this cell into account and ignore it in our count.
        neighborMatrix[0, 0] = 1; neighborMatrix[0, 1] = 0; neighborMatrix[0, 2] = 0;
        neighborMatrix[1, 0] = 0; neighborMatrix[1, 1] = BoolToInt(alive); neighborMatrix[1, 2] = 0;
        neighborMatrix[2, 0] = 1; neighborMatrix[2, 1] = 0; neighborMatrix[2, 2] = 0;

        amountOfNeighbors = CountNeighbors(neighborMatrix);

        print(amountOfNeighbors);
        //The order of these are important as one state being true might make other states unreachable.
        //We have to make sure that every state is reachable with the given value conditions.
        //we to not need to check the alive state of <2 or >3 since either scenario dead or alive would result in that status being set to false/not alive.
        //We do need to check the alive state for == 3 and >=2 since it matters wether those values should be handled when alive or dead.
        if (amountOfNeighbors < 2)
        {
            alive = false;
            print("Not enough neighbors alive to survive. The Cell dies of lonelyness :(." + alive);


        }
        //This one should only be checked if the cell is not alive as it controlls rebrith.
        else if (amountOfNeighbors == 3 && !alive)
        {
            alive = true;
            print("Exactly 3 neighbors found. Congratulations the Cell had a baby. (Dead cell turned back to life)." + alive);
        }
        else if (amountOfNeighbors > 3)
        {
            alive = false;
            print("3 or more neighbors are alive. Cell dies of over population." + alive);
        }
        //we need to specifically check if the cell is alive to prevent dead cells from being revived.
        else if (amountOfNeighbors >= 2 && alive)
        {
            alive = true;
            print("2 or more neighbors are alive. Cell survives :D." + alive);

        }





    }



    int CountNeighbors(int[,] neighbormatrix)
    {
        int counter = 0;
        for (int row = 0; row < neighborMatrix.GetLength(0); row++)
        {
            for (int collum = 0; collum < neighborMatrix.GetLength(1); collum++)
            {

                if (neighborMatrix[row, collum] != neighborMatrix[1, 1])
                {
                    counter += neighborMatrix[row, collum];
                    print(counter);

                }

            }
        }




        print(counter);
        return counter;

    }

    int BoolToInt(bool boolVal)
    {
        return boolVal ? 1 : 0;
    }

    //the 2d array represents a 3x3 matrix. Typing it out in the formatting shown below helps us visualize the matrix that will be stored in the array.
    ///////
    //100//
    //010//
    //001//
    //////

}