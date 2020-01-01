//TODO(용택): (Corridor) 각각의 방 Room 을 연결하는 통로를 건설한다.

//NOTE(용택): 대충 찾아보니 이런 모양을 원할 때는 Delaunay 제네레이션 알고리즘을 이용한다고 한다. BSP 보다 한 3배쯤은 힘들다는 듯.
//          비슷한 구현이 필요할 때 이 방식을 택하도록 한다.
//NOTE(용택): Room = Node(Vertex) , Corridor = Edge 로 보는 그래프라고 할 수 있다.
//          (1) 우선 Node 만 있을 뿐 Edge 가 전무하기 때문에 그래프를 먼저 건설해야한다.
//              ==> Node 는 Rect 형태로 볼 수 있으므로, Rough-Centroid(BSP의 좌표와 거의 같은..) 들의 거리쌍을 구할 수 있다.
//                  이를 "기본" 그래프로 이용한다. 모든 정점이 모든 정점으로 연결되므로 Node n 개에 대해 n(n-1) 개 Edge 가 생성된다.
//          (2) 최소신장트리(Minimum Spanning Tree) 등을 이용해서 서브그래프를 생성한다.
//              여기에는 Nearest Neighbor 중 1~x 개를 선택하는 방법을 이용한다.             
//                      --> 4 방향으로 이어지길 원하니, 1~4 정도로 한다.
//              ==> 서브그래프를 어떻게 생성할 것인가?
//                  1. 전체 2darray(=맵) 의 Centroid 에 해당하는 Node 에서부터 점진적 생성
//                  2. Start, End 에서 1개씩 간선을 선택하고, 양 끝에서부터 점진적 생성
//                  3. 일단 Nearest-Pair 를 먼저 잇고, 나머지를 순차적으로 결정하는 Sollin 알고리즘 스타일*
//                          --> 이 방법이 무난해 보인다.
//NOTE(용택): 이렇게 생성된 "서브그래프" 의 Edge 를 기반으로 실제 Corridor 를 좌표상으로 만들어낸다.
//          복수개의 Room(Node) 이 좌표가 겹치거나/인접하는 경우 하나의 Room(Node)으로 보도록 처리해두었으므로,
//          Map 상에서는 Node 와 Node 사이에는 최소한 1개의 빈 Cell 이 존재한다.
//NOTE(용택): Corridor를 채울 때의 주의점
//          (1) Nearest Neighbor 를 연결할 때, 대각선 방향으로 향하는 수가 있다.
//                      --> 필터가 필요할 수 있다. (각도..등으로, 되려나...)
//          (2) 이미 존재하는 Corridor 에 대해 겹침이 발생할 수 있다.
//                      --> 사실 큰 문제는 없다. 갈래길로 생각할 수 있으므로.
//                      --> 가장 큰 문제는 연결이 '안될 때' 다. 케이스가 발생하는 지는 해보고 고치자.


using System;
using System.Collections.Generic;

namespace minorlife
{
    public class Corridor
    {
        class Edge
        {
            int[,] edges = null;
            int numVertex = 0;
        }
        //..
    }
}