#############################
# CCycles wrapper
#############################

set(INC
  ..
)
set(INC_SYS
)

set(LIB
  cycles_device
  cycles_kernel
  cycles_scene
  cycles_session
  cycles_bvh
  cycles_subd
  cycles_graph
  cycles_util
)

#file( GLOB SRCS *.h *.cpp)
#add_library( ccycles STATIC ${SRCS} )

add_definitions(-DCCL_CAPI_DLL)

#set_target_properties(ccycles PROPERTIES LINKER_LANGUAGE CXX)
#target_include_directories(ccycles PRIVATE ../cycles/src)
#target_include_directories(ccycles PRIVATE ../../../../../../big_libs/OpenImageIO-2.2.19/include)
#target_include_directories(ccycles PRIVATE ../../../../../../big_libs/OpenEXR-2.5.5/include)

if(WITH_ALEMBIC)
  add_definitions(-DWITH_ALEMBIC)
  list(APPEND INC_SYS
    ${ALEMBIC_INCLUDE_DIRS}
  )
  list(APPEND LIB
    ${ALEMBIC_LIBRARIES}
  )
endif()

if(WITH_CYCLES_OSL)
  list(APPEND LIB cycles_kernel_osl)
endif()

if(CYCLES_STANDALONE_REPOSITORY)
  list(APPEND LIB extern_sky)
else()
  list(APPEND LIB bf_intern_sky)
endif()

if(WITH_CYCLES_STANDALONE AND WITH_CYCLES_STANDALONE_GUI)
  list(APPEND INC_SYS
    ${Epoxy_INCLUDE_DIRS}
    ${SDL2_INCLUDE_DIRS}
  )
  list(APPEND LIB ${Epoxy_LIBRARIES} ${SDL2_LIBRARIES})
endif()

if(WITH_USD)
  # Silence warning from USD headers using deprecated TBB header.
  add_definitions(
    -D__TBB_show_deprecation_message_atomic_H
    -D__TBB_show_deprecation_message_task_H
  )

  list(APPEND INC_SYS
    ${USD_INCLUDE_DIRS}
  )
  list(APPEND LIB
    cycles_hydra
    ${USD_LIBRARIES}
  )
endif()

cycles_external_libraries_append(LIB)

# Common configuration.

include_directories(${INC})
include_directories(SYSTEM ${INC_SYS})

# Application build targets

if(WITH_CYCLES_STANDALONE)
  file( GLOB SRC *.cpp *.h *.c)

  add_library(ccycles SHARED ${SRC} ${INC} ${INC_SYS})
  unset(SRC)

  target_link_libraries(ccycles PRIVATE ${LIB})

  if(APPLE)
    if(WITH_CYCLES_STANDALONE_GUI)
      set_property(TARGET ccycles APPEND_STRING PROPERTY LINK_FLAGS
        " -framework Cocoa -framework CoreAudio -framework AudioUnit -framework AudioToolbox -framework ForceFeedback -framework CoreVideo")
    endif()
  endif()

  if(UNIX AND NOT APPLE)
    set_target_properties(ccycles PROPERTIES INSTALL_RPATH $ORIGIN/lib)
  endif()

  if(CYCLES_STANDALONE_REPOSITORY)
    cycles_install_libraries(ccycles)
  endif()

  if(WITH_USD AND USD_LIBRARY_DIR)
    install(DIRECTORY
      ${USD_LIBRARY_DIR}/usd
      DESTINATION ${CMAKE_INSTALL_PREFIX}
    )

    install(DIRECTORY
      ${USD_LIBRARY_DIR}/../plugin/usd
      DESTINATION ${CMAKE_INSTALL_PREFIX}
    )
  endif()

  install(PROGRAMS
    $<TARGET_FILE:ccycles>
    DESTINATION ${CMAKE_INSTALL_PREFIX})
endif()
