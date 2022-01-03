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
    [SerializeField]
    int[,,] neighborMatrix = new int[3, 3, 3];




    //We store the neighbors in a 3x3 boolean matrix, for each "alive" neighbor we count +1


    void Start()
    {
        //Add the cell to our event updater as soon as the game starts.
        SystemUpdater.instance.ONupdateEvent += UpdateThisCell;


    }

    public void RandomizeCell(bool randomize)
    {
        if (randomize)
        {
            alive = IntToBool(Random.Range(0, 2));
            GetComponent<MeshRenderer>().enabled = alive;
        }

    }

    public void UpdateThisCell()
    {

        CheckNeighbors();
        GetComponent<MeshRenderer>().enabled = alive;
    }

    [Button]
    void CheckNeighbors()
    {
        //Note that the middle value of the whole matrix represents our cell. We should not take this cell into account and ignore it in our count.
        //Use the below reference to figure out how the array is stored. This is important when checking neighbors to not get out of bounds.
        //The matrix represents a 2d slice.
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
        else if (amountOfNeighbors == 3)
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
        else if (amountOfNeighbors >= 2)
        {
            alive = true;
            print("2 or more neighbors are alive. Cell survives :D." + alive);

        }





    }
    //Although we have a 3D array, the first test will leave the z paramter on 0 so that we work in a 2D manner to simplefy testing currently.
    [Button]
    int[,,] GetNeighbors(GameObject[,,] gridList)
    {
        //listInex is passed on by te cell its stored index and should refence its own object in the gridlist matrix.
        //do not forget to increase the z length ofthis array once we implement a 3d matrix.
        int[,,] neighborList = new int[3, 3, 3];


        //Calculation offset needed based on the current cell's index to get its neighbors.
        /*         [-1, -1, 0][0, +1, 0][+1, -1, 0] = 0;
                  [-1, 0, 0][1, 1, 0][+1, 0, 0] = 0;
                  [-1, +1, 0][0, +1, 0][+1, +1, 0] = 0;*/
        #region 
        //We have to check whether the grid index has a valid object to call to prevent errors. might as well do it inline.
        //It might look a bit crowded. But at the moment I could not find a way to write these lines any shorter. I opted for inline ifstatements too keep my sanity with the document length.
        //Since we support a 3D grid, we will have to check the current cell's slice, the slice in front and the slice behind him. We can simply copy the matrix for the regular slice and change the z index to + or -.
        /*   neighborList[0, 0, 0] = ArrayHasIndex(gridList, new Vector3Int(listIndex.x - 1, listIndex.y - 1, listIndex.z - 1)) != false ? BoolToInt(gridList[listIndex.x - 1, listIndex.y - 1, listIndex.z - 1].GetComponent<Cell_Behaviour>().alive) : 0;
           neighborList[1, 0, 0] = ArrayHasIndex(gridList, new Vector3Int(listIndex.x, listIndex.y - 1, listIndex.z - 1)) != false ? BoolToInt(gridList[listIndex.x, listIndex.y - 1, listIndex.z - 1].GetComponent<Cell_Behaviour>().alive) : 0;
           neighborList[2, 0, 0] = ArrayHasIndex(gridList, new Vector3Int(listIndex.x + 1, listIndex.y - 1, listIndex.z - 1)) != false ? BoolToInt(gridList[listIndex.x + 1, listIndex.y - 1, listIndex.z - 1].GetComponent<Cell_Behaviour>().alive) : 0;
           neighborList[0, 1, 0] = ArrayHasIndex(gridList, new Vector3Int(listIndex.x - 1, listIndex.y, listIndex.z - 1)) != false ? BoolToInt(gridList[listIndex.x - 1, listIndex.y, listIndex.z - 1].GetComponent<Cell_Behaviour>().alive) : 0;
           neighborList[1, 1, 0] = ArrayHasIndex(gridList, new Vector3Int(listIndex.x, listIndex.y, listIndex.z - 1)) != false ? BoolToInt(gridList[listIndex.x, listIndex.y, listIndex.z - 1].GetComponent<Cell_Behaviour>().alive) : 0;
           neighborList[2, 1, 0] = ArrayHasIndex(gridList, new Vector3Int(listIndex.x + 1, listIndex.y, listIndex.z - 1)) != false ? BoolToInt(gridList[listIndex.x + 1, listIndex.y, listIndex.z - 1].GetComponent<Cell_Behaviour>().alive) : 0;
           neighborList[0, 2, 0] = ArrayHasIndex(gridList, new Vector3Int(listIndex.x - 1, listIndex.y + 1, listIndex.z - 1)) != false ? BoolToInt(gridList[listIndex.x - 1, listIndex.y + 1, listIndex.z - 1].GetComponent<Cell_Behaviour>().alive) : 0;
           neighborList[1, 2, 0] = ArrayHasIndex(gridList, new Vector3Int(listIndex.x, listIndex.y + 1, listIndex.z - 1)) != false ? BoolToInt(gridList[listIndex.x, listIndex.y + 1, listIndex.z - 1].GetComponent<Cell_Behaviour>().alive) : 0;
           neighborList[2, 2, 0] = ArrayHasIndex(gridList, new Vector3Int(listIndex.x + 1, listIndex.y + 1, listIndex.z + -1)) != false ? BoolToInt(gridList[listIndex.x + 1, listIndex.y + 1, listIndex.z - 1].GetComponent<Cell_Behaviour>().alive) : 0;

           neighborList[0, 0, 1] = ArrayHasIndex(gridList, new Vector3Int(listIndex.x - 1, listIndex.y - 1, listIndex.z)) != false ? BoolToInt(gridList[listIndex.x - 1, listIndex.y - 1, listIndex.z].GetComponent<Cell_Behaviour>().alive) : 0;
           neighborList[1, 0, 1] = ArrayHasIndex(gridList, new Vector3Int(listIndex.x, listIndex.y - 1, listIndex.z)) != false ? BoolToInt(gridList[listIndex.x, listIndex.y - 1, listIndex.z].GetComponent<Cell_Behaviour>().alive) : 0;
           neighborList[2, 0, 1] = ArrayHasIndex(gridList, new Vector3Int(listIndex.x + 1, listIndex.y - 1, listIndex.z)) != false ? BoolToInt(gridList[listIndex.x + 1, listIndex.y - 1, listIndex.z].GetComponent<Cell_Behaviour>().alive) : 0;
           neighborList[0, 1, 1] = ArrayHasIndex(gridList, new Vector3Int(listIndex.x - 1, listIndex.y, listIndex.z)) != false ? BoolToInt(gridList[listIndex.x - 1, listIndex.y, listIndex.z].GetComponent<Cell_Behaviour>().alive) : 0;
           neighborList[1, 1, 1] = 0;//This value is always 0 as it represents the current cell itself and should never be taken into your calculation.
           neighborList[2, 1, 1] = ArrayHasIndex(gridList, new Vector3Int(listIndex.x + 1, listIndex.y, listIndex.z)) != false ? BoolToInt(gridList[listIndex.x + 1, listIndex.y, listIndex.z].GetComponent<Cell_Behaviour>().alive) : 0;
           neighborList[0, 2, 1] = ArrayHasIndex(gridList, new Vector3Int(listIndex.x - 1, listIndex.y + 1, listIndex.z)) != false ? BoolToInt(gridList[listIndex.x - 1, listIndex.y + 1, listIndex.z].GetComponent<Cell_Behaviour>().alive) : 0;
           neighborList[1, 2, 1] = ArrayHasIndex(gridList, new Vector3Int(listIndex.x, listIndex.y + 1, listIndex.z)) != false ? BoolToInt(gridList[listIndex.x, listIndex.y + 1, listIndex.z].GetComponent<Cell_Behaviour>().alive) : 0;
           neighborList[2, 2, 1] = ArrayHasIndex(gridList, new Vector3Int(listIndex.x + 1, listIndex.y + 1, listIndex.z)) != false ? BoolToInt(gridList[listIndex.x + 1, listIndex.y + 1, listIndex.z].GetComponent<Cell_Behaviour>().alive) : 0;

           neighborList[0, 0, 2] = ArrayHasIndex(gridList, new Vector3Int(listIndex.x - 1, listIndex.y - 1, listIndex.z + 1)) != false ? BoolToInt(gridList[listIndex.x - 1, listIndex.y - 1, listIndex.z + 1].GetComponent<Cell_Behaviour>().alive) : 0;
           neighborList[1, 0, 2] = ArrayHasIndex(gridList, new Vector3Int(listIndex.x, listIndex.y - 1, listIndex.z + 1)) != false ? BoolToInt(gridList[listIndex.x, listIndex.y - 1, listIndex.z + 1].GetComponent<Cell_Behaviour>().alive) : 0;
           neighborList[2, 0, 2] = ArrayHasIndex(gridList, new Vector3Int(listIndex.x + 1, listIndex.y - 1, listIndex.z + 1)) != false ? BoolToInt(gridList[listIndex.x + 1, listIndex.y - 1, listIndex.z + 1].GetComponent<Cell_Behaviour>().alive) : 0;
           neighborList[0, 1, 2] = ArrayHasIndex(gridList, new Vector3Int(listIndex.x - 1, listIndex.y, listIndex.z + 1)) != false ? BoolToInt(gridList[listIndex.x - 1, listIndex.y, listIndex.z + 1].GetComponent<Cell_Behaviour>().alive) : 0;
           neighborList[1, 1, 2] = ArrayHasIndex(gridList, new Vector3Int(listIndex.x, listIndex.y, listIndex.z + 1)) != false ? BoolToInt(gridList[listIndex.x, listIndex.y, listIndex.z + 1].GetComponent<Cell_Behaviour>().alive) : 0;
           neighborList[2, 1, 2] = ArrayHasIndex(gridList, new Vector3Int(listIndex.x + 1, listIndex.y, listIndex.z + 1)) != false ? BoolToInt(gridList[listIndex.x + 1, listIndex.y, listIndex.z + 1].GetComponent<Cell_Behaviour>().alive) : 0;
           neighborList[0, 2, 2] = ArrayHasIndex(gridList, new Vector3Int(listIndex.x - 1, listIndex.y + 1, listIndex.z + 1)) != false ? BoolToInt(gridList[listIndex.x - 1, listIndex.y + 1, listIndex.z + 1].GetComponent<Cell_Behaviour>().alive) : 0;
           neighborList[1, 2, 2] = ArrayHasIndex(gridList, new Vector3Int(listIndex.x, listIndex.y + 1, listIndex.z + 1)) != false ? BoolToInt(gridList[listIndex.x, listIndex.y + 1, listIndex.z + 1].GetComponent<Cell_Behaviour>().alive) : 0;
           neighborList[2, 2, 2] = ArrayHasIndex(gridList, new Vector3Int(listIndex.x + 1, listIndex.y + 1, listIndex.z + 1)) != false ? BoolToInt(gridList[listIndex.x + 1, listIndex.y + 1, listIndex.z + 1].GetComponent<Cell_Behaviour>().alive) : 0;

   */
        #endregion
        //top left                                                                               //top                                                                                  //top right
        SetNeighBorListValue(ref neighborList, new Vector3Int(-1, -1, 0), gridList, listIndex); SetNeighBorListValue(ref neighborList, new Vector3Int(0, -1, 0), gridList, listIndex); SetNeighBorListValue(ref neighborList, new Vector3Int(1, -1, 0), gridList, listIndex);
        //left                                                                                   //mid/cell itself                                                                      //right
        SetNeighBorListValue(ref neighborList, new Vector3Int(-1, 0, 0), gridList, listIndex); SetNeighBorListValue(ref neighborList, new Vector3Int(0, 0, 0), gridList, listIndex); SetNeighBorListValue(ref neighborList, new Vector3Int(1, 0, 0), gridList, listIndex);
        //bottom left                                                                            //bottom                                                                               //bottom right
        SetNeighBorListValue(ref neighborList, new Vector3Int(-1, 1, 0), gridList, listIndex); SetNeighBorListValue(ref neighborList, new Vector3Int(0, 1, 0), gridList, listIndex); SetNeighBorListValue(ref neighborList, new Vector3Int(1, 1, 0), gridList, listIndex);



        return neighborList;
    }

    void SetNeighBorListValue(ref int[,,] neighborList, Vector3Int indexOffset, GameObject[,,] gridList, Vector3Int listIndex)
    {
        neighborList[1 + indexOffset.x, 1 + indexOffset.y, 1 + indexOffset.z] = ArrayHasIndex(gridList, listIndex + indexOffset) ? BoolToInt(gridList[listIndex.x + indexOffset.x, listIndex.y + indexOffset.y, listIndex.z + indexOffset.z].GetComponent<Cell_Behaviour>().alive) : 0;

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

                    // }

                }
            }
        }



        amountOfNeighbors = counter;
        return counter;

    }



    //Helper methods.
    int BoolToInt(bool boolVal)
    {
        return boolVal == true ? 1 : 0;
    }
    bool IntToBool(int intVal)
    {
        return intVal == 1 ? true : false;
    }



    //Checks if an index actually exist in an array. (Really wish this was nativally supported)
    bool ArrayHasIndex(GameObject[,,] gridList, Vector3Int index)
    {
        //only return true if the index value fits within the 3d array bounds.
        if (index.x >= 0 && index.x < gridList.GetLength(0) && index.y >= 0 && index.y < gridList.GetLength(1) && index.z >= 0 && index.z < gridList.GetLength(2))
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