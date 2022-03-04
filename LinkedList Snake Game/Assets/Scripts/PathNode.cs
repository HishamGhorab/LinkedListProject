public class PathNode
{
    public int x;
    public int y;
    
    public PathNode cameFromNode;
    
    private PathNode[,] nodeGrid;

    public PathNode(PathNode[,] grid, int givenX, int givenY)
    {
        nodeGrid = grid;
        x = givenX;
        y = givenY;
    }
}
