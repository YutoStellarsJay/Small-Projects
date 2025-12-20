#include <iostream>
#include <fstream>
#include <vector>

int main()
{
    // I made this program because a friend challenged me to make """"""""""machine learning"""""""""" in C++ with training data below 1kb
    std::vector<char> key;
    std::vector<double> value;
    std::ifstream read("cpp_data.txt");
    std::string line;
    while (std::getline(read, line))
    {
        key.push_back(line[0]);
        value.push_back(std::stod(line.substr(1, line.size() - 1)));
    }
    std::string input;
    std::cout << "Input a value for this to sum the chances of its letter probabilities";
    std::cin >> input;
    double sum = 0.0;
    for (int i = 0; i < key.size(); ++i)
    {
        for (char j : input)
        {
            if (key[i] == j)
            {
                sum += value[i];
            }
        }
    }
    double theme = 0;
    for (char c : input)
    {
        for (int i = 0; i < key.size(); ++i)
        {
            if (key[i] == c)
            {
                theme += std::log((double)(value[i] + 1.0) / (double)(key.size() + 1.0));
            }
        }
    }
    std::cout << "theme " << theme << '\n';
}