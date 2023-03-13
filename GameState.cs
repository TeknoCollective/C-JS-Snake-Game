//Class which stores the game state.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;

namespace Snake
{
    public class GameState
    {

        //Setting up the variables needed to store the GameState.
        public int Rows { get; }
        public int Cols { get; }
        public GridValue[,] Grid { get; }
        public Direction Dir { get; private set; }
        public int Score { get; private set; }
        public bool GameOver { get; private set; }

        private readonly LinkedList<Direction> dirChanges = new LinkedList<Direction>();
        //Grab Current position of the Snake.
        public readonly LinkedList<Position> snakePositions = new LinkedList<Position>();

        //Create an array to randomly generate where the food should spawn.
        public readonly Random random = new Random();

        //Constructor to take number of rows and colums in grid as parameters.
        public GameState(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;

            //Initialise array with specified size.
            Rows = rows;
            Cols = cols;
            Grid = new GridValue[rows, cols];
            Dir = Direction.Right;

            AddSnake();
            AddFood();
        }

        //Create method to add the snake to the grid.

        private void AddSnake()
        {
            int r = Rows / 2;

            //Loop over the columns from 1 - 3.
            for (int c = 1; c <= 3; c++)
            {
                Grid[r, c] =  GridValue.Snake;
                snakePositions.AddFirst(new Position(r, c));
            }
        }

        //Loop through all rows and colums.
        //Inside the loop we check if grid is empty if yes return it.
        private IEnumerable<Position> EmptyPositions()
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (Grid[r, c] == GridValue.Empty)
                    {
                        yield return new Position(r, c);

                    }
                }
            }
        }

        //Method which adds the food to the grid.
        private void AddFood()
        {
            //Create a list of all empty positions.
            List<Position> empty = new List<Position>(EmptyPositions());

            // If snake killed no empty=, this code avoids a crash.
            if (empty.Count == 0)
            {
                return;

            }

            //Pick empty position at random.
            Position pos = empty[random.Next(empty.Count)];

            //Set array value.
            Grid[pos.Row, pos.Col] = GridValue.Food;

        }

        //Grab snake position from linked list.
        public Position HeadPosition()
        {
            return snakePositions.First.Value;

        }

        public Position TailPosition()
        {
            return snakePositions.Last.Value;

        }

        //Get all snake positions.
        public IEnumerable<Position> SnakePositions()
        {
            return snakePositions;

        }

        //Modify the Snake
        public void AddHead(Position pos)
        {
            snakePositions.AddFirst(pos);
            Grid[pos.Row, pos.Col] = GridValue.Snake;

        }

        public void RemoveTail()
        {
            Position tail = snakePositions.Last.Value;
            Grid[tail.Row, tail.Col] = GridValue.Empty;
            snakePositions.RemoveLast();

        }

        private Direction GetLastDirection()
        {
            if (dirChanges.Count == 0)
            {
                return Dir;
            }
            
                return dirChanges.Last.Value;
        }

        private bool CanChangeDirection(Direction newDir)
        {
            if (dirChanges.Count == 2)
            {
                return false;
            }

            Direction lastDir = GetLastDirection();
            return newDir != lastDir && newDir != lastDir.Opposite();
        }

        //Change direction of snake.
        public void ChangeDirection(Direction dir)
        {

            //check if change can be made in direction.

            if (CanChangeDirection(dir))
            {
                dirChanges.AddLast(dir);

            }

           
           
        }

        //Check if the snake is outside the grid.
        public bool OutsideGrid (Position pos)
        {
            return pos.Row < 0 || pos.Row >= Rows || pos.Col < 0 || pos.Col >= Cols;

        }
        private GridValue WillHit(Position newHeadPos)
        {

            if (OutsideGrid(newHeadPos))
            {
                return GridValue.Outside;
            }
            if (newHeadPos == TailPosition())
            {
                return GridValue.Empty;
            }

            return Grid[newHeadPos.Row, newHeadPos.Col];
        }

        //Move the snake one step in the current direction.
        public void Move()
        {

            if (dirChanges.Count > 0)
            {
                Dir = dirChanges.First.Value;
                dirChanges.RemoveFirst();
            }

            Position newHeadPos = HeadPosition().Translate(Dir);
            GridValue hit = WillHit(newHeadPos);

            if (hit == GridValue.Outside || hit == GridValue.Snake)
            {
                GameOver = true;

            }
            else if (hit == GridValue.Empty)
            {
                RemoveTail();
                AddHead(newHeadPos);
            }
            else if (hit == GridValue.Food)
            {
                AddHead(newHeadPos);
                Score++;
                AddFood();

            }

        }
    }
}
