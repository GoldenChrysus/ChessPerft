using PonzianiComponents.Chesslib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimHanewich.Chess;

namespace ChessPerft {
    internal class Program {
        static PonzianiComponents.Chesslib.Game game_ponz;
        static TimHanewich.Chess.BoardPosition game_tim;

        static readonly Dictionary<string, Dictionary<int, int>> expected = new Dictionary<string, Dictionary<int, int>>() {
            {
                "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1",
                new Dictionary<int, int>() {
                    { 1, 20 },
                    { 2, 400 },
                    { 3, 8902 },
                    { 4, 197281 },
                    { 5, 4865609 },
                    { 6, 119060324 }
                }
            },
            {
                "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - ",
                new Dictionary<int, int>() {
                    { 1, 48 },
                    { 2, 2039 },
                    { 3, 97862 },
                    { 4, 4085603 },
                    // { 5, 193690690 }
                }
            },
            {
                "8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - ",
                new Dictionary<int, int>() {
                    { 1, 14 },
                    { 2, 191 },
                    { 3, 2812 },
                    { 4, 43238 },
                    { 5, 674624 },
                    { 6, 11030083 }
                }
            },
            {
                "r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1",
                new Dictionary<int, int>() {
                    { 1, 6 },
                    { 2, 264 },
                    { 3, 9467 },
                    { 4, 422333 },
                    { 5, 15833292 }
                }
            },
            {
                "rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8",
                new Dictionary<int, int>() {
                    { 1, 44 },
                    { 2, 1486 },
                    { 3, 62379 },
                    { 4, 2103487 },
                    { 5, 89941194 }
                }
            },
            {
                "r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10",
                new Dictionary<int, int>() {
                    { 1, 46 },
                    { 2, 2079 },
                    { 3, 89890 },
                    { 4, 3894594 },
                    // { 5, 164075551 }
                }
            }
        };

        static void Main(string[] args) {
            int suite_num = 0;
            int fails = 0;

            Console.Write("Choose library to test: (1. Ponziani, 2. TimHanewich): ");

            string test = Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine("\nBEGINNING ALL TESTS\n");

            foreach (string fen in expected.Keys) {
                int depth = Math.Min(expected[fen].Keys.Last(), 5);

                Console.ForegroundColor = ConsoleColor.Blue;

                if (++suite_num > 1) {
                    Console.WriteLine();
                }

                Console.WriteLine("Suite {0}: {1}", suite_num, fen);
                Console.WriteLine("Max Depth: {0}\n", depth);

                for (int i = 1; i <= depth; i++) {
                    game_ponz = new PonzianiComponents.Chesslib.Game(fen);
                    game_tim = new TimHanewich.Chess.BoardPosition(fen);
                    int count;

                    switch (test) {
                        case "1":
                            count = GenerateMovesPonziani(i);

                            break;

                        case "2":
                            count = GenerateMovesTim(i);

                            break;

                        default:
                            count = 0;

                            break;
                    }

                    bool pass = (count == expected[fen][i]);

                    if (!pass) {
                        fails++;
                    }

                    Console.ForegroundColor = (pass) ? ConsoleColor.Green : ConsoleColor.Red;

                    string expectation = (pass) ? "" : " (expected: {3})";

                    Console.WriteLine(
                        "Depth: {0}   Positions: {1}   Pass: {2}" + expectation, i, count, pass, expected[fen][i]
                    );
                }
            }

            Console.ForegroundColor = (fails == 0) ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed;

            Console.WriteLine();
            Console.WriteLine((fails == 0) ? "ALL TESTS PASSED" : "{0} TEST(S) FAILED", fails);
            Console.ReadLine();
        }

        static int GenerateMovesPonziani(int depth) {
            List<PonzianiComponents.Chesslib.Move> moves = game_ponz.Position.GetMoves();
            int position_count = 0;

            if (depth == 1) {
                return moves.Count;
            }

            string fen = game_ponz.Position.FEN;

            foreach (PonzianiComponents.Chesslib.Move move in moves) {
                game_ponz.Add(new PonzianiComponents.Chesslib.ExtendedMove(move));

                position_count += GenerateMovesPonziani(depth - 1);

                game_ponz = new Game(fen);
            }

            return position_count;
        }

        static int GenerateMovesTim(int depth = 1) {
            TimHanewich.Chess.Move[] moves = game_tim.AvailableMoves();
            int position_count = 0;

            if (depth == 1) {
                return moves.Length;
            }

            string fen = game_tim.ToFEN();

            for (int i = 0; i < moves.Length; i++) {
                game_tim.ExecuteMove(moves[i]);

                position_count += GenerateMovesTim(depth - 1);

                game_tim = new BoardPosition(fen);
            }

            return position_count;
        }
    }
}
