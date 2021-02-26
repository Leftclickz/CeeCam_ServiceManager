# CeeCam_ServiceManager
Work project for managing services and running them with a designated timer. Once the managed service is started, it will be automatically stopped once the timer interval is reached.

# How to install:

The project must first be compiled. Once conpiled, install the CeeCam_ServiceManager service using the following command while within the directory of your executable:
> `installutils CeeCam_ServiceManager.exe`

To uninstall use the following command:
> `installutils /u CeeCam_ServiceManager.exe`

# How to start:

The program requires 2 parameters before initializing.

- --Service {service_name} : this is the service being manipulated by CeeCam_ServiceManager
- --Interval (milliseconds) : this is the interval used by the timer that automatically starts/stops the managed service

To start CeeCam_ServiceManager run using the following command:

> `sc start CeeCam_ServiceManager --Service {service_name} --Interval {milliseconds}`

After running start it will take a moment before the service begins. To confirm it is running use the following command:

> `sc query CeeCam_ServiceManager`

Display info looks as follows:  

SERVICE_NAME: CeeCam_ServiceManager  
DISPLAY_NAME: CeeCam_ServiceManager  
             TYPE                        : 10 WIN32_OWN_PROCESS  
             STATE                       : 4 RUNNING  
                                             (STOPPABLE, NOT_PAUSABLE, ACCEPTS_SHUTDOWN)  
             WIN32_EXIT_CODE             : 0 (0x0)  
             SERVICE_EXIT_CODE           : 0 (0x0)  
             CHECKPOINT                  : 0x0  
             WAIT_HINT                   : 0x0  
            
 If STATE is not RUNNING or RUNNINGPENDING then your parameters were incorrect and the service was unable to start.
             

# How to stop:

To stop CeeCam_ServiceManager run the following command:

> `sc stop CeeCam_ServiceManager`

# How to Use CeeCam_ServiceManager:

To run a subcommand use the following command:

> `sc control CeeCam_ServiceManager {control value}`

Control values:

- 128 -- Starts the timer and runs the service if it is not already running.

To change the service or timer being used you must first stop CeeCam_ServiceManager and then start it again using new parameters.
