#ifndef __MRS_FILE_HPP__
#define __MRS_FILE_HPP__

#include <mrs.hpp>
#include <sys/stat.h>

#ifndef MRS_WINDOWS
# include <unistd.h>
#endif

namespace mrs {
class File
{
public:
    static bool Remove( const char* path ){
        return ( 0 == remove( path ) );
    }
    
    static bool IsExist( const char* path ){
        struct stat info;
        return ( 0 == stat( path, &info ) );
    }
    
    static std::string ReadToString( const char* path ){
        File file;
        file.Open( path, "rb" );
        mrs::Buffer buffer;
        char data[ 10240 ];
        do{
            uint32 read_len = file.Read( data, sizeof( data ) );
            if ( 0 == read_len ) break;
            
            buffer.Write( data, read_len );
        }while ( true );
        file.Close();
        buffer.WriteInt8( '\0' );
        buffer.Unwrite( 1 );
        return std::string( buffer.GetData(), buffer.GetDataLen() );
    }
    
protected:
    FILE*       m_File;
    std::string m_FilePath;
    
public:
    std::string GetFilePath(){ return m_FilePath; }
    
public:
    File(){
        m_File = NULL;
    }
    
    virtual ~File(){
        Close();
    }
    
    virtual bool Open( const char* path, const char* mode ){
        if ( NULL != m_File ) Close();
        
#ifdef MRS_WINDOWS
        fopen_s( &m_File, path, mode );
#else
        m_File = fopen( path, mode );
#endif
        if ( NULL != m_File ){
            m_FilePath = path;
        }
        return ( NULL != m_File );
    }
    
    virtual void Close(){
        if ( NULL != m_File ){
            fclose( m_File );
            m_File = NULL;
        }
    }
    
    virtual void Reopen(){
        std::string file_path = m_FilePath;
        Open( file_path.c_str(), "a" );
    }
    
    virtual void Printf( const char* format, va_list list ){
        if ( NULL != m_File ) vfprintf( m_File, format, list );
    }
    
    virtual void Printf( const char* format, ... ){
        va_list list;
        va_start( list, format );
        Printf( format, list );
        va_end( list );
    }
    
    virtual void Flush(){
        if ( NULL != m_File ) fflush( m_File );
    }
    
    virtual uint32 Write( const void* data, uint32 data_len ){
        return ( NULL == m_File ) ? 0 : (uint32)fwrite( data, 1, data_len, m_File );
    }
    
    virtual uint32 Read( void* data, uint32 data_len ){
        return ( NULL == m_File ) ? 0 : (uint32)fread( data, 1, data_len, m_File );
    }
};
};

#endif
