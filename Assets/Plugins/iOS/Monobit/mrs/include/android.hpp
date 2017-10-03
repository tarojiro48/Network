#ifndef __MRS_ANDROID__
#define __MRS_ANDROID__

#if ( 19 < __ANDROID_API__ )
extern "C" {
sighandler_t bsd_signal( int value, sighandler_t callback ){
    struct sigaction new_act;
    struct sigaction old_act;
    memset( &new_act, 0, sizeof( new_act ) );
    new_act.sa_handler = callback;
    sigaction( value, &new_act, &old_act );
    return old_act.sa_handler;
}
}
#endif

#endif

