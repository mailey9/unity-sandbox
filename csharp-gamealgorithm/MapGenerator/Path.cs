using System;
using System.Collections.Generic;

namespace minorlife
{
    public class Path
    {

    }
    //TODO(용택): 구현, Room 을 하나의 노드로 보고, 두 노드를 잇는다.

    //  Start to End 가 여러 Path 인 건 관계없다.
    //  4-dir 중 한 방향 이상으로 연결 해야한다.
    //  하지만, "경로 있음" 으로 connectivity 가 보장 되어야 한다.

    //  CellType 이 int 로 되어있지만, Tile 로 Representation 되야 한다.

    //NOTE(용택): Thoughts.
    //  I. Start/End 가 될 노드를 pick 한다 (경우에 따라 같은 노드 일 수 있다.)
    //  II. %로 보존할 노드 외에 나머지는 discard
    //  III. %로 입구포인트를 결정한다. (대충 20%~80% 위치, rect 의 col 또는 row 포인트)
    //  IV. Closest Node 를 우선 pick 한다. (dist 로 판별할 수 있다.)
    //
}