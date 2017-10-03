#ifndef __MRS_UTILITY_HPP__
#define __MRS_UTILITY_HPP__

#include <common.hpp>

namespace mrs {
class Utility
{
public:
    static std::string ToHex( const void* data, uint32 data_len );
};
};

#endif
