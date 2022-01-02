using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;

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
    int[,,] neighborMatrix = new int[3, 3, 0];

    //We store the neighbors in a 3x3 boolean matrix, for each "alive" neighbor we count +1


    void Start()
    {

        SystemUpdater.instance._updateEvent.AddListener(UpdateThisCell);
    }


    void UpdateThisCell()
    {
        GetComponent<MeshRenderer>().enabled = CheckNeighbors();
    }

    [Button]
    bool CheckNeighbors()
    {
        //At the moment this matrix is filled with test data. However later-on we will have to base these values on the neighbor cell's alive state where true = 1 and false = 0.
        //Note that the middle value of the whole matrix represents our cell. We should take this cell into account and ignore it in our count.
        //Use the below reference to figure out how the array is stored. This is important when checking neighbors to not get out of bounds.
        /*[0, 0, 0][1, 0, 0][2 , 0, 0] = 0;
          [0, 1, 0][1, 1, 0][2, 1, 0] = 0;
          [0, 2, 0][1, 2, 0][2, 2, 0] = 0;*/

        neighborMatrix = GetNeighbors(GenerateGrid.instance.gridListRead);
        amountOfNeighbors = CountNeighbors(neighborMatrix);

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

        return alive;



    }
    //Although we have a 3D array, the first test will leave the z paramter on 0 so that we work in a 2D manner to simplefy testing currently.
    [Button]
    int[,,] GetNeighbors(GameObject[,,] gridList)
    {

        //listInex is passed on by te cell its stored index and should refence its own object in the gridlist matrix.
        int[,,] neighborList = new int[3, 3, 1];

        //Calculation offset needed based on the current cell's index to get its neighbors.
        /*         [-1, -1, 0][0, +1, 0][+1, -1, 0] = 0;
                  [-1, 0, 0][1, 1, 0][+1, 0, 0] = 0;
                  [-1, +1, 0][0, +1, 0][+1, +1, 0] = 0;*/

        //We have to check whether the grid index has a valid object to call to prevent errors. might as well do it inline.
        //It might look a bit crowded. But at the moment I could not find a way to write these lines any shorter. I opted for inline ifstatements too keep my sanity with the document length.
        neighborList[0, 0, 0] = ArrayHasIndex(gridList, new Vector3Int(listIndex.x - 1, listIndex.y - 1, listIndex.z)) != false ? BoolToInt(gridList[listIndex.x - 1, listIndex.y - 1, listIndex.z].GetComponent<Cell_Behaviour>().alive) : 0;
        neighborList[1, 0, 0] = ArrayHasIndex(gridList, new Vector3Int(listIndex.x, listIndex.y - 1, listIndex.z)) != false ? BoolToInt(gridList[listIndex.x, listIndex.y - 1, listIndex.z].GetComponent<Cell_Behaviour>().alive) : 0;
        neighborList[2, 0, 0] = ArrayHasIndex(gridList, new Vector3Int(listIndex.x + 1, listIndex.y - 1, listIndex.z)) != false ? BoolToInt(gridList[listIndex.x + 1, listIndex.y - 1, listIndex.z].GetComponent<Cell_Behaviour>().alive) : 0;
        neighborList[0, 1, 0] = ArrayHasIndex(gridList, new Vector3Int(listIndex.x - 1, listIndex.y, listIndex.z)) != false ? BoolToInt(gridList[listIndex.x - 1, listIndex.y, listIndex.z].GetComponent<Cell_Behaviour>().alive) : 0;
        neighborList[1, 1, 0] = 0;//This value is always 0 as it represents the current cell itself and should never be taken into your calculation.
        neighborList[2, 1, 0] = ArrayHasIndex(gridList, new Vector3Int(listIndex.x + 1, listIndex.y, listIndex.z)) != false ? BoolToInt(gridList[listIndex.x + 1, listIndex.y, listIndex.z].GetComponent<Cell_Behaviour>().alive) : 0;
        neighborList[0, 2, 0] = ArrayHasIndex(gridList, new Vector3Int(listIndex.x - 1, listIndex.y + 1, listIndex.z)) != false ? BoolToInt(gridList[listIndex.x - 1, listIndex.y + 1, listIndex.z].GetComponent<Cell_Behaviour>().alive) : 0;
        neighborList[1, 2, 0] = ArrayHasIndex(gridList, new Vector3Int(listIndex.x, listIndex.y + 1, listIndex.z)) != false ? BoolToInt(gridList[listIndex.x, listIndex.y + 1, listIndex.z].GetComponent<Cell_Behaviour>().alive) : 0;
        neighborList[2, 2, 0] = ArrayHasIndex(gridList, new Vector3Int(listIndex.x + 1, listIndex.y + 1, listIndex.z)) != false ? BoolToInt(gridList[listIndex.x + 1, listIndex.y + 1, listIndex.z].GetComponent<Cell_Behaviour>().alive) : 0;




        //print(BoolToInt(gridList[listIndex.x - 1, listIndex.y - 1, listIndex.z].GetComponent<Cell_Behaviour>().alive));
        return neighborList;
    }


    int CountNeighbors(int[,,] neighborList)
    {
        int counter = 0;
        for (int depth = 0; depth < neighborList.GetLength(2); depth++)
        {
            for (int collum = 0; collum < neighborList.GetLength(1); collum++)
            {
                for (int row = 0; row < neighborList.GetLength(0); row++)
                {


                    // {
                    counter += neighborList[row, collum, depth];
                    print(counter);

                    // }

                }
            }
        }




        print(counter);
        return counter;

    }



    //Helper methods.
    int BoolToInt(bool boolVal)
    {
        return boolVal ? 1 : 0;
    }



    //Checks if an index actually exist in an array. (Really wish this was nativally supported)
    bool ArrayHasIndex(GameObject[,,] array, Vector3Int index)
    {
        //only return true if the index value fits within the 3d array bounds.
        if (index.x >= 0 && index.x < array.GetLength(0) && index.y >= 0 && index.y < array.GetLength(1) && index.z >= 0 && index.z < array.GetLength(2))
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    //the 2d array represents a 3x3 matrix. Typing it out in the formatting shown below helps us visualize the matrix that will be stored in the array.
    ///////
    //100//
    //010//
    //001//
    //////

}