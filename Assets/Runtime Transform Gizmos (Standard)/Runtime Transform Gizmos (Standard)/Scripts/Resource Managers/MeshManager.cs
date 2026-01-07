using UnityEngine;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: MeshManager (Public Singleton Class)
    // Desc: Manages a collection of meshes used throughout the plugin and implements
    //       relevant utility functions.
    // Defs: Unit Box           - box width width, height & depth = 1.
    //       Unit Quad          - quad with width & height = 1.
    //       Unit Cylinder      - cylinder with length and radius = 1.
    //       Unit Cone          - cone with length and radius = 1.
    //       Unit Segment       - segment with length = 1.
    //       Unit Sphere        - sphere with radius = 1.
    //       Unit Pyramid       - pyramid with a base size = 1 and height = 1.
    //       Unit Torus         - torus with a tube radius = 1 and radius = 1.
    //       Unit RATriangle    - right-angled triangle with both its adjacent sides = 1.
    //-----------------------------------------------------------------------------
    public class MeshManager : Singleton<MeshManager>
    {
        #region Private Fields
        Mesh mUnitXYRATriangle;         // Unit XY right-angled triangle
        Mesh mUnitWireXYRATriangle;     // Unit wire XY right-angled triangle

        Mesh mUnitBox;                  // Unit box
        Mesh mUnitWireBox;              // Unit wire box
        Mesh mUnitBoxXSegment;          // Unit box that can can be used a unit segment that starts from the origin and is aligned with the X axis
        Mesh mUnitWireBoxXSegment;      // Unit wire box that can can be used a unit segment that starts from the origin and is aligned with the X axis

        Mesh mUnitCylinderXCap;         // Unit cylinder that caps the X axis
        Mesh mUnitZCylinder;            // Unit cylinder whose length axis is aligned with the Z axis
        Mesh mUnitConeXCap;             // Unit cone that caps the X axis

        Mesh mUnitSphere;               // Unit sphere
        Mesh mUnitZTorus;               // Unit torus whose main axis is aligned with the Z axis

        Mesh mUnitSPyramidXCap;         // Unit square pyramid that caps the X axis
        Mesh mUnitWireSPyramidXCap;     // Unit wire square pyramid that caps the X axis

        Mesh mUnitXYCircle;             // Unit XY circle
        Mesh mUnitWireXYCircle;         // Unit wire XY circle
        Mesh mUnitWireYZCircle;         // Unit wire YZ circle
        Mesh mUnitWireZXCircle;         // Unit wire ZX circle

        Mesh mUnitXYQuad;               // Unit XY quad
        Mesh mUnitWireXYQuad;           // Unit wire XY quad
        Mesh mUnitZXQuad;               // Unit ZX quad
        Mesh mSpriteQuad;               // Sprite quad
        Mesh mBlitQuad;                 // Blit quad mesh used for blit operations

        Mesh mUnitXSegment;             // A unit segment that starts from the origin and is aligned with the X axis
        Mesh mUnitYSegment;             // A unit segment that starts from the origin and is aligned with the Y axis
        Mesh mUnitZSegment;             // A unit segment that starts from the origin and is aligned with the Z axis
        #endregion

        #region Public Properties
        //-----------------------------------------------------------------------------
        // Name: unitXYRATriangle (Public Property)
        // Desc: Returns a unit right-angled triangle that sits in the XY plane.
        //-----------------------------------------------------------------------------
        public Mesh unitXYRATriangle
        {
            get
            {
                if (mUnitXYRATriangle == null) 
                    mUnitXYRATriangle = MeshEx.CreateRATriangle(new RATriangleMeshDesc { trianglePlane = EFlatMeshPlane.XY, aeSize0 = 1.0f, aeSize1 = 1.0f, color = Color.white });
                return mUnitXYRATriangle;
            }
        }

        //-----------------------------------------------------------------------------
        // Name: unitWireXYRATriangle (Public Property)
        // Desc: Returns a wire unit right-angled triangle that sits in the XY plane.
        //-----------------------------------------------------------------------------
        public Mesh unitWireXYRATriangle
        {
            get
            {
                if (mUnitWireXYRATriangle == null) 
                    mUnitWireXYRATriangle = MeshEx.CreateWireRATriangle(new RATriangleMeshDesc { trianglePlane = EFlatMeshPlane.XY, aeSize0 = 1.0f, aeSize1 = 1.0f, color = Color.white });
                return mUnitWireXYRATriangle;
            }
        }

        //-----------------------------------------------------------------------------
        // Name: unitBox (Public Property)
        // Desc: Returns a unit box mesh.
        //-----------------------------------------------------------------------------
        public Mesh unitBox
        {
            get
            {
                if (mUnitBox == null) mUnitBox = MeshEx.CreateBox(new BoxMeshDesc { width = 1.0f, height = 1.0f, depth = 1.0f, color = Color.white });
                return mUnitBox;
            }
        }

        //-----------------------------------------------------------------------------
        // Name: unitWireBox (Public Property)
        // Desc: Returns a unit wire box mesh.
        //-----------------------------------------------------------------------------
        public Mesh unitWireBox
        {
            get
            {
                if (mUnitWireBox == null) mUnitWireBox = MeshEx.CreateWireBox(new BoxMeshDesc { width = 1.0f, height = 1.0f, depth = 1.0f, color = Color.white });
                return mUnitWireBox;
            }
        }

        //-----------------------------------------------------------------------------
        // Name: unitBoxXSegment (Public Property)
        // Desc: Returns a unit box that can can be used a unit segment that starts from
        //       the origin and is aligned with the X axis.
        //-----------------------------------------------------------------------------
        public Mesh unitBoxXSegment
        {
            get
            {
                if (mUnitBoxXSegment == null) mUnitBoxXSegment = MeshEx.CreateBox(new BoxMeshDesc { width = 1.0f, height = 1.0f, depth = 1.0f, color = Color.white, center = new Vector3(0.5f, 0.0f, 0.0f) });
                return mUnitBoxXSegment;
            }
        }

        //-----------------------------------------------------------------------------
        // Name: unitWireBoxXSegment (Public Property)
        // Desc: Returns a unit wire box that can can be used a unit segment that starts 
        //       from the origin and is aligned with the X axis.
        //-----------------------------------------------------------------------------
        public Mesh unitWireBoxXSegment
        {
            get
            {
                if (mUnitWireBoxXSegment == null) mUnitWireBoxXSegment = MeshEx.CreateWireBox(new BoxMeshDesc { width = 1.0f, height = 1.0f, depth = 1.0f, color = Color.white, center = new Vector3(0.5f, 0.0f, 0.0f) });
                return mUnitWireBoxXSegment;
            }
        }

        //-----------------------------------------------------------------------------
        // Name: unitCylinderXCap (Public Property)
        // Desc: Returns a unit cylinder that caps the X axis.
        //-----------------------------------------------------------------------------
        public Mesh unitCylinderXCap
        {
            get
            {
                if (mUnitCylinderXCap == null)
                {
                    // Fill descriptor
                    CylinderMeshDesc desc = new CylinderMeshDesc();
                    desc.length         = 1.0f;
                    desc.radius         = 1.0f;
                    desc.lengthAxis     = 0;
                    desc.sliceCount     = 30;
                    desc.stackCount     = 1;
                    desc.capRingCount0  = 1;
                    desc.capRingCount1  = 1;
                    desc.color          = Color.white;

                    // Create mesh
                    mUnitCylinderXCap = MeshEx.CreateCylinder(desc);
                }
                return mUnitCylinderXCap;
            }
        }

        //-----------------------------------------------------------------------------
        // Name: unitZCylinder (Public Property)
        // Desc: Returns a unit cylinder that caps the X axis.
        //-----------------------------------------------------------------------------
        public Mesh unitZCylinder
        {
            get
            {
                if (mUnitZCylinder == null)
                {
                    // Fill descriptor
                    CylinderMeshDesc desc = new CylinderMeshDesc();
                    desc.length         = 1.0f;
                    desc.radius         = 1.0f;
                    desc.lengthAxis     = 2;
                    desc.sliceCount     = 30;
                    desc.stackCount     = 1;
                    desc.capRingCount0  = 1;
                    desc.capRingCount1  = 1;
                    desc.color          = Color.white;

                    // Create mesh
                    mUnitZCylinder = MeshEx.CreateCylinder(desc);
                }
                return mUnitZCylinder;
            }
        }

        //-----------------------------------------------------------------------------
        // Name: unitConeXCap (Public Property)
        // Desc: Returns a unit cylinder whose length axis is aligned with the Z axis.
        //-----------------------------------------------------------------------------
        public Mesh unitConeXCap
        {
            get
            {
                if (mUnitConeXCap == null)
                {
                    // Fill descriptor
                    ConeMeshDesc desc = new ConeMeshDesc();
                    desc.length         = 1.0f;
                    desc.radius         = 1.0f;
                    desc.lengthAxis     = 0;
                    desc.sliceCount     = 20;
                    desc.stackCount     = 1;
                    desc.capRingCount   = 1;
                    desc.color          = Color.white;

                    // Create mesh
                    mUnitConeXCap   = MeshEx.CreateCone(desc);
                }
                return mUnitConeXCap;
            }
        }
        
        //-----------------------------------------------------------------------------
        // Name: unitSphere (Public Property)
        // Desc: Returns a unit sphere.
        //-----------------------------------------------------------------------------
        public Mesh unitSphere
        {
            get
            {
                if (mUnitSphere == null)
                {
                    // Fill descriptor
                    SphereMeshDesc desc = new SphereMeshDesc();
                    desc.radius         = 1.0f;
                    desc.sliceCount     = 30;
                    desc.stackCount     = 30;
                    desc.color          = Color.white;

                    // Create mesh
                    mUnitSphere   = MeshEx.CreateSphere(desc);
                }
                return mUnitSphere;
            }
        }

        //-----------------------------------------------------------------------------
        // Name: unitZTorus (Public Property)
        // Desc: Returns a unit torus whose main axis is aligned with the Z axis.
        //-----------------------------------------------------------------------------
        public Mesh unitZTorus
        {
            get
            {
                if (mUnitZTorus == null)
                {
                    // Fill descriptor
                    TorusMeshDesc desc      = new TorusMeshDesc();
                    desc.radius             = 1.0f;
                    desc.tubeRadius         = 1.0f;
                    desc.sliceCount         = 80;
                    desc.crossSliceCount    = 80;
                    desc.mainAxis           = 2;

                    // Create mesh
                    mUnitZTorus = MeshEx.CreateTorus(desc);
                }
                return mUnitZTorus;
            }
        }

        //-----------------------------------------------------------------------------
        // Name: unitSPyramidXCap (Public Property)
        // Desc: Returns a unit square pyramid that caps the X axis.
        //-----------------------------------------------------------------------------
        public Mesh unitSPyramidXCap
        {
            get
            {
                if (mUnitSPyramidXCap == null)
                {
                    // Fill descriptor
                    SquarePyramidMeshDesc desc = new SquarePyramidMeshDesc();
                    desc.length         = 1.0f;
                    desc.baseSize       = 1.0f;
                    desc.lengthAxis     = 0;
                    desc.color          = Color.white;

                    // Create mesh
                    mUnitSPyramidXCap   = MeshEx.CreateSquarePyramid(desc);
                }

                return mUnitSPyramidXCap;
            }
        }

        //-----------------------------------------------------------------------------
        // Name: unitWireSPyramidXCap (Public Property)
        // Desc: Returns a unit wire square pyramid that caps the X axis.
        //-----------------------------------------------------------------------------
        public Mesh unitWireSPyramidXCap
        {
            get
            {
                if (mUnitWireSPyramidXCap == null)
                {
                    // Fill descriptor
                    SquarePyramidMeshDesc desc = new SquarePyramidMeshDesc();
                    desc.length         = 1.0f;
                    desc.baseSize       = 1.0f;
                    desc.lengthAxis     = 0;
                    desc.color          = Color.white;

                    // Create mesh
                    mUnitWireSPyramidXCap   = MeshEx.CreateWireSquarePyramid(desc);
                }

                return mUnitWireSPyramidXCap;
            }
        }

        //-----------------------------------------------------------------------------
        // Name: unitXYCircle (Public Property)
        // Desc: Returns a unit XY circle.
        //-----------------------------------------------------------------------------
        public Mesh unitXYCircle
        {
            get
            {
                if (mUnitXYCircle == null) mUnitXYCircle = MeshEx.CreateCircle(new CircleMeshDesc { circlePlane = EFlatMeshPlane.XY, radius = 1.0f, sliceCount = 80, color = Color.white });
                return mUnitXYCircle;
            }
        }

        //-----------------------------------------------------------------------------
        // Name: unitWireXYCircle (Public Property)
        // Desc: Returns a unit wire XY circle.
        //-----------------------------------------------------------------------------
        public Mesh unitWireXYCircle
        {
            get
            {
                if (mUnitWireXYCircle == null) mUnitWireXYCircle = MeshEx.CreateWireCircle(new CircleMeshDesc { circlePlane = EFlatMeshPlane.XY, radius = 1.0f, sliceCount = 80, color = Color.white });
                return mUnitWireXYCircle;
            }
        }

        //-----------------------------------------------------------------------------
        // Name: unitWireYZCircle (Public Property)
        // Desc: Returns a unit wire YZ circle.
        //-----------------------------------------------------------------------------
        public Mesh unitWireYZCircle
        {
            get
            {
                if (mUnitWireYZCircle == null) mUnitWireYZCircle = MeshEx.CreateWireCircle(new CircleMeshDesc { circlePlane = EFlatMeshPlane.YZ, radius = 1.0f, sliceCount = 80, color = Color.white });
                return mUnitWireYZCircle;
            }
        }

        //-----------------------------------------------------------------------------
        // Name: unitWireZXCircle (Public Property)
        // Desc: Returns a unit wire ZX circle.
        //-----------------------------------------------------------------------------
        public Mesh unitWireZXCircle
        {
            get
            {
                if (mUnitWireZXCircle == null) mUnitWireZXCircle = MeshEx.CreateWireCircle(new CircleMeshDesc { circlePlane = EFlatMeshPlane.ZX, radius = 1.0f, sliceCount = 80, color = Color.white });
                return mUnitWireZXCircle;
            }
        }

        //-----------------------------------------------------------------------------
        // Name: unitXYQuad (Public Property)
        // Desc: Returns a unit XY quad.
        //-----------------------------------------------------------------------------
        public Mesh unitXYQuad
        {
            get
            {
                if (mUnitXYQuad == null) mUnitXYQuad = MeshEx.CreateQuad(new QuadMeshDesc { quadPlane = EFlatMeshPlane.XY, width = 1.0f, height = 1.0f, color = Color.white });
                return mUnitXYQuad;
            }
        }

        //-----------------------------------------------------------------------------
        // Name: unitWireXYQuad (Public Property)
        // Desc: Returns a unit wire XY quad.
        //-----------------------------------------------------------------------------
        public Mesh unitWireXYQuad
        {
            get
            {
                if (mUnitWireXYQuad == null) mUnitWireXYQuad = MeshEx.CreateWireQuad(new QuadMeshDesc { quadPlane = EFlatMeshPlane.XY, width = 1.0f, height = 1.0f, color = Color.white });
                return mUnitWireXYQuad;
            }
        }

        //-----------------------------------------------------------------------------
        // Name: unitZXQuad (Public Property)
        // Desc: Returns a unit ZX quad.
        //-----------------------------------------------------------------------------
        public Mesh unitZXQuad
        {
            get 
            {
                if (mUnitZXQuad == null) mUnitZXQuad = MeshEx.CreateQuad(new QuadMeshDesc(){ quadPlane = EFlatMeshPlane.ZX, width = 1.0f, height = 1.0f, color = Color.white });
                return mUnitZXQuad;
            }
        }

        //-----------------------------------------------------------------------------
        // Name: spriteQuad (Public Property)
        // Desc: Returns a mesh that can be used to render sprite quads.
        //-----------------------------------------------------------------------------
        public Mesh spriteQuad
        {
            get
            {
                if (mSpriteQuad == null) mSpriteQuad = MeshEx.CreateQuad(new QuadMeshDesc(){ quadPlane = EFlatMeshPlane.XY, width = 1.0f, height = 1.0f, color = Color.white });
                return mSpriteQuad;
            }
        }

        //-----------------------------------------------------------------------------
        // Name: blitQuad (Public Property)
        // Desc: Returns a mesh that can be used to perform blit operations. This is
        //       a quad whose vertices cover the entire viewport area.
        //-----------------------------------------------------------------------------
        public Mesh blitQuad
        {
            get
            {
                if (mBlitQuad == null) mBlitQuad = MeshEx.CreateBlitQuad();
                return mBlitQuad;
            }
        }

        //-----------------------------------------------------------------------------
        // Name: unitXSegment (Public Property)
        // Desc: Returns a unit length segment aligned with the X axis.
        //-----------------------------------------------------------------------------
        public Mesh unitXSegment
        {
            get
            {
                if (mUnitXSegment == null) mUnitXSegment = MeshEx.CreateSegment(Vector3.zero, Vector3.right, Color.white);
                return mUnitXSegment;
            }
        }

        //-----------------------------------------------------------------------------
        // Name: unitYSegment (Public Property)
        // Desc: Returns a unit length segment aligned with the Y axis.
        //-----------------------------------------------------------------------------
        public Mesh unitYSegment
        {
            get
            {
                if (mUnitYSegment == null) mUnitYSegment = MeshEx.CreateSegment(Vector3.zero, Vector3.up, Color.white);
                return mUnitYSegment;
            }
        }

        //-----------------------------------------------------------------------------
        // Name: unitZSegment (Public Property)
        // Desc: Returns a unit length segment aligned with the Z axis.
        //-----------------------------------------------------------------------------
        public Mesh unitZSegment
        {
            get
            {
                if (mUnitZSegment == null) mUnitZSegment = MeshEx.CreateSegment(Vector3.zero, Vector3.forward, Color.white);
                return mUnitZSegment;
            }
        }
        #endregion
    }
    #endregion
}
