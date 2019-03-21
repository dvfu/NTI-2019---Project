#include <fstream>
#include <vector>
#include <iostream>
#include <set>
#include <map>

using namespace std;

int main(int argc, char** argv)
{
	ifstream finIn(argv[1]);
	ifstream fin1(argv[2]);
	ifstream fin2(argv[3]);

	int n1, n2;

	fin1 >> n1;
	fin2 >> n2;

	if (n1 != n2)
	{
		cout << "Wrong count of resources";
		return 1;
	}

	map<int, map<int, std::map<int, int>>> r1;
	map<int, map<int, std::map<int, int>>> r2;

	for (int i = 0; i < n1; i++)
	{
		float x1, y1;
		float x2, y2;
		int id1, id2;

		fin1 >> x1 >> y1 >> id1;
		fin2 >> x2 >> y2 >> id2;

		int rx1, ry1, rx2, ry2;

		rx1 = round(x1 - 0.5f);
		ry1 = round(y1 - 0.5f);
		rx2 = round(x2 - 0.5f);
		ry2 = round(y2 - 0.5f);

		r1[rx1][ry1][id1]++;
		r2[rx2][ry2][id2]++;
	}

	for (const auto& ys : r1)
	{
		for (const auto& cnts: ys.second)
		{
			for (const auto& id : cnts.second)
			{
				if (!r2.count(ys.first) || !r2[ys.first].count(cnts.first) || !r2[ys.first][cnts.first].count(id.first) || r2[ys.first][cnts.first][id.first] != id.second)
				{
					cout << "Resource " << id.second << " at position " << ys.first + 0.5f << ", " << cnts.first + 0.5f << " is not found";
					return 1;
				}
			}
		}
	}

	return 0;
}