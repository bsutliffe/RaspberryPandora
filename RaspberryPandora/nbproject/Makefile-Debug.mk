#
# Generated Makefile - do not edit!
#
# Edit the Makefile in the project folder instead (../Makefile). Each target
# has a -pre and a -post target defined where you can add customized code.
#
# This makefile implements configuration specific macros and targets.


# Environment
MKDIR=mkdir
CP=cp
GREP=grep
NM=nm
CCADMIN=CCadmin
RANLIB=ranlib
CC=gcc
CCC=g++
CXX=g++
FC=gfortran
AS=as

# Macros
CND_PLATFORM=GNU-MacOSX
CND_DLIB_EXT=dylib
CND_CONF=Debug
CND_DISTDIR=dist
CND_BUILDDIR=build

# Include project Makefile
include Makefile

# Object Directory
OBJECTDIR=${CND_BUILDDIR}/${CND_CONF}/${CND_PLATFORM}

# Object Files
OBJECTFILES= \
	${OBJECTDIR}/Blowfish/BlowFish.o \
	${OBJECTDIR}/APIWrapper.o \
	${OBJECTDIR}/Main.o


# C Compiler Flags
CFLAGS=

# CC Compiler Flags
CCFLAGS=-lcurl
CXXFLAGS=-lcurl

# Fortran Compiler Flags
FFLAGS=

# Assembler Flags
ASFLAGS=

# Link Libraries and Options
LDLIBSOPTIONS=-L/usr/local/lib -ljson_linux-gcc-4.2.1_libmt

# Build Targets
.build-conf: ${BUILD_SUBPROJECTS}
	"${MAKE}"  -f nbproject/Makefile-${CND_CONF}.mk ${CND_DISTDIR}/${CND_CONF}/${CND_PLATFORM}/raspberrypandora

${CND_DISTDIR}/${CND_CONF}/${CND_PLATFORM}/raspberrypandora: ${OBJECTFILES}
	${MKDIR} -p ${CND_DISTDIR}/${CND_CONF}/${CND_PLATFORM}
	${LINK.cc} -o ${CND_DISTDIR}/${CND_CONF}/${CND_PLATFORM}/raspberrypandora ${OBJECTFILES} ${LDLIBSOPTIONS} 

${OBJECTDIR}/Blowfish/BlowFish.o: Blowfish/BlowFish.cpp 
	${MKDIR} -p ${OBJECTDIR}/Blowfish
	${RM} $@.d
	$(COMPILE.cc) -g -I/usr/local/include -MMD -MP -MF $@.d -o ${OBJECTDIR}/Blowfish/BlowFish.o Blowfish/BlowFish.cpp

${OBJECTDIR}/APIWrapper.o: APIWrapper.cpp 
	${MKDIR} -p ${OBJECTDIR}
	${RM} $@.d
	$(COMPILE.cc) -g -I/usr/local/include -MMD -MP -MF $@.d -o ${OBJECTDIR}/APIWrapper.o APIWrapper.cpp

${OBJECTDIR}/Main.o: Main.cpp 
	${MKDIR} -p ${OBJECTDIR}
	${RM} $@.d
	$(COMPILE.cc) -g -I/usr/local/include -MMD -MP -MF $@.d -o ${OBJECTDIR}/Main.o Main.cpp

# Subprojects
.build-subprojects:

# Clean Targets
.clean-conf: ${CLEAN_SUBPROJECTS}
	${RM} -r ${CND_BUILDDIR}/${CND_CONF}
	${RM} ${CND_DISTDIR}/${CND_CONF}/${CND_PLATFORM}/raspberrypandora

# Subprojects
.clean-subprojects:

# Enable dependency checking
.dep.inc: .depcheck-impl

include .dep.inc