
using ARSoft.Tools.Net.Dns;
using Org.BouncyCastle.Math

namespace ArsoftTestServer.DnsSec
{


    public class WireParseException : System.Exception
    {
        public WireParseException(string msg)
        { }
    }


    public class DNSInput
    {
        public DNSInput()
        { }

        public DNSInput(byte[] key)
        { }


        //private byte[] byteBuffer;

        private ArsoftTestServer.JavaUtilities.ByteBuffer byteBuffer;

        // Returns the number of bytes that can be read from this stream before reaching the end. 
        public int remaining()
        {
            return byteBuffer.Remaining();
        }

        private void require(int n) // throws WireParseException
        {
            if (n > remaining())
            {
                throw new WireParseException("end of input");
            }
        }


        public int readU8() // throws WireParseException
        {
            require(1);
            return (byteBuffer.Get() & 0xFF);
        }

        /**
         * Reads an unsigned 16 bit value from the stream, as an int.
         *
         * @return An unsigned 16 bit value.
         * @throws WireParseException The end of the stream was reached.
         */
        public int readU16() // throws WireParseException
        {
            require(2);
            return (byteBuffer.GetShort() & 0xFFFF);
        }

        /**
         * Reads an unsigned 32 bit value from the stream, as a long.
         *
         * @return An unsigned 32 bit value.
         * @throws WireParseException The end of the stream was reached.
         */
        public long readU32() // throws WireParseException
        {
            require(4);
            return (byteBuffer.GetInt() & 0xFFFFFFFFL);
        }

        /**
         * Reads a byte array consisting of the remainder of the stream (or the active region, if one is
         * set.
         *
         * @return The byte array.
         */
        public byte[] readByteArray()
        {
            int len = remaining();
            byte[] @out = new byte[len];
            byteBuffer.Get(@out, 0, len);
            return @out;
        }


        /**
         * Reads a byte array of a specified length from the stream into an existing array.
         *
         * @param b The array to read into.
         * @param off The offset of the array to start copying data into.
         * @param len The number of bytes to copy.
         * @throws WireParseException The end of the stream was reached.
         */
        public void readByteArray(byte[] b, int off, int len) // throws WireParseException
        {
            require(len);
            byteBuffer.Get(b, off, len);
        }

        /**
         * Reads a byte array of a specified length from the stream.
         *
         * @return The byte array.
         * @throws WireParseException The end of the stream was reached.
         */
        public byte[] readByteArray(int len) // throws WireParseException
        {
            require(len);
            byte[] @out = new byte[len];
            byteBuffer.Get(@out, 0, len);
            return @out;
        }

    } // End Class DNSInput




    public class IllegalArgumentException
        :System.Exception
    {

        public IllegalArgumentException()
        { }

        public IllegalArgumentException(string message)
        { }

    }



    public class DNSOutput
    {

        private byte[] array;
        private int pos;
        private int saved_pos;

        // Create a new DNSOutput with a specified size.
        // @param size The initial size
        public DNSOutput(int size)
        {
            array = new byte[size];
            pos = 0;
            saved_pos = -1;
        }

        // Create a new DNSOutput
        public DNSOutput()
            : this(32)
        { }


        public byte[] toByteArray()
        {
            return array;
        }


        private void check(long val, int bits)
        {
            long max = 1;
            max <<= bits;
            if (val < 0 || val > max)
            {
                throw new IllegalArgumentException(val + " out of range for " + bits + " bit value");
            }
        }

        private void need(int n)
        {
            if (array.Length - pos >= n)
            {
                return;
            }
            int newsize = array.Length * 2;
            if (newsize < pos + n)
            {
                newsize = pos + n;
            }

            byte[] newarray = new byte[newsize];
            System.Array.Copy(array, 0, newarray, 0, pos);
            array = newarray;
        }


        // Writes an unsigned 8 bit value to the stream.
        // @param val The value to be written
        public void writeU8(int val)
        {
            check(val, 8);
            need(1);
            array[pos++] = (byte)(val & 0xFF);
        }


        // Writes an unsigned 16 bit value to the stream.
        // @param val The value to be written
        public void writeU16(int val)
        {
            check(val, 16);
            need(2);
            array[pos++] = (byte)((val >>> 8) & 0xFF);
            array[pos++] = (byte)(val & 0xFF);
        }

        /**
        * Writes a byte array to the stream.
        *
        * @param b The array to write.
        * @param off The offset of the array to start copying data from.
        * @param len The number of bytes to write.
        */
        public void writeByteArray(byte[] b, int off, int len)
        {
            need(len);
            System.Array.Copy(b, off, array, pos, len);
            pos += len;
        }

        // Writes a byte array to the stream.
        // @param b The array to write.
        public void writeByteArray(byte[] b)
        {
            writeByteArray(b, 0, b.Length);
        }
    }


    public class PublicKey
    {

    }


    public class RSAPublicKey
        : PublicKey
    {

    }

    public class DSAPublicKey
        : PublicKey
    {

    }

    public class ECPublicKey
        : PublicKey
    {

    }


    public class IncompatibleKeyException : System.Exception
    { }

    public class UnsupportedAlgorithmException : System.Exception
    {

        public UnsupportedAlgorithmException(int alg)
        { }
    }

    public class MalformedKeyException : System.Exception
    {
        public MalformedKeyException(KEYBase r)
        { }

        public MalformedKeyException(KEYBase r, System.IO.IOException e)
        { }
    }



    public class DNSSECException : System.Exception
    {

        public DNSSECException()
        { }

        public DNSSECException(Org.BouncyCastle.Security.GeneralSecurityException e)
        { }
    }



    // KEYBase.java
    public class KEYBase
    {

        // Returns the binary data representing the key
        public byte[] getKey()
        {
            // return key;
            return null;
        }

        public int getAlgorithm()
        {
            return 123;
        }




    }



    // https://gist.github.com/wuyongzheng/0e2ed6d8a075153efcd3
    public class ECKeyInfo
    {
        public int length;
        // public BigInteger p, a, b, gx, gy, n;
        public Org.BouncyCastle.Math.BigInteger p, a, b, gx, gy, n;
        // private EllipticCurve curve;
        public Org.BouncyCastle.Math.EC.ECCurve curve;
        //private ECParameterSpec spec;
        public Org.BouncyCastle.Crypto.Parameters.ECDomainParameters spec;


        private static BigInteger FromHex(string hex)
        {
            return new BigInteger(1, Org.BouncyCastle.Utilities.Encoders.Hex.Decode(hex));
        }


        public ECKeyInfo(
            int length,
            string p_str,
            string a_str,
            string b_str,
            string gx_str,
            string gy_str,
            string n_str)
        {
            this.length = length;
            p = new BigInteger(p_str, 16);
            a = new BigInteger(a_str, 16);
            b = new BigInteger(b_str, 16);
            gx = new BigInteger(gx_str, 16);
            gy = new BigInteger(gy_str, 16);
            n = new BigInteger(n_str, 16);
            BigInteger h = BigInteger.One;

            this.curve = new Org.BouncyCastle.Math.EC.FpCurve(p, a, b, n, h);

            //spec = new ECParameterSpec(curve, new ECPoint(gx, gy), n, 1);
            this.spec = new Org.BouncyCastle.Crypto.Parameters.ECDomainParameters(
                 curve
                ,curve.CreatePoint(gx, gy) // G 
                ,n
                ,h
            );

        }


        // Those are hex values ! (prepend 0x)

        // https://github.com/benvanik/openssl/blob/master/openssl/engines/ccgost/gost_params.c
        // https://github.com/google/jalic/blob/master/openssl-lwekex/engines/ccgost/gost_params.h
        // ==> NID_id_GostR3410_2001_CryptoPro_A_ParamSet,
        // RFC 4357 Section 11.4
        public static ECKeyInfo GOST =
            new ECKeyInfo(
          32, // nid 
          "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFD97", // a
          "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFD94", // b
          "A6", // p 
          "1", // q
          "8D91E471E0989CDA27DF505A453F2B7635294F2DDF23E3B122ACC99C9E9F1E14", // x
          "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF6C611070995AD10045841B09B761B893" // y
        );

        // RFC 5114 Section 2.6
        // https://github.com/openssl/openssl/issues/4970
        // EC_GFp_nistp256_method
        // Org.BouncyCastle.Math.EC.Custom.Sec.SecP256R1Curve
        // Org.BouncyCastle.Asn1.Sec.SecNamedCurves.Secp256r1Holder
        public static ECKeyInfo ECDSA_P256 =
            new ECKeyInfo(
          32,
          "FFFFFFFF00000001000000000000000000000000FFFFFFFFFFFFFFFFFFFFFFFF", // p 
          "FFFFFFFF00000001000000000000000000000000FFFFFFFFFFFFFFFFFFFFFFFC", // a
          "5AC635D8AA3A93E7B3EBBD55769886BC651D06B0CC53B0F63BCE3C3E27D2604B", // b
          "6B17D1F2E12C4247F8BCE6E563A440F277037D812DEB33A0F4A13945D898C296", // Gx
          "4FE342E2FE1A7F9B8EE7EB4A7C0F9E162BCE33576B315ECECBB6406837BF51F5", // Gy
          "FFFFFFFF00000000FFFFFFFFFFFFFFFFBCE6FAADA7179E84F3B9CAC2FC632551" // n 
        );

        // RFC 5114 Section 2.7
        // namespace Org.BouncyCastle.Math.EC.Custom.Sec.SecP384R1Curve
        // Org.BouncyCastle.Asn1.Sec.SecNamedCurves.Secp384r1Holder
        public static ECKeyInfo ECDSA_P384 =
            new ECKeyInfo(
          48,
          "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFFFFF0000000000000000FFFFFFFF", // p 
          "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFFFFF0000000000000000FFFFFFFC", // a 
          "B3312FA7E23EE7E4988E056BE3F82D19181D9C6EFE8141120314088F5013875AC656398D8A2ED19D2A85C8EDD3EC2AEF", // b 
          "AA87CA22BE8B05378EB1C71EF320AD746E1D3B628BA79B9859F741E082542A385502F25DBF55296C3A545E3872760AB7", // Gx
          "3617DE4A96262C6F5D9E98BF9292DC29F8F41DBD289A147CE9DA3113B5F0B8C00A60B1CE1D7E819D7A431D7C90EA0E5F", // Gy
          "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFC7634D81F4372DDF581A0DB248B0A77AECEC196ACCC52973" // n 
        );


        private static PublicKey toECGOSTPublicKey(KEYBase r, ECKeyInfo keyinfo) // throws IOException, GeneralSecurityException 
        {
            DNSInput @in = new DNSInput(r.getKey());

            // BigInteger x = readBigIntegerLittleEndian(@in, keyinfo.length);
            // BigInteger y = readBigIntegerLittleEndian(@in, keyinfo.length);
            // ECPoint q = new ECPoint(x, y);

            // KeyFactory factory = KeyFactory.getInstance("ECGOST3410");
            // return factory.generatePublic(new ECPublicKeySpec(q, keyinfo.spec));
            return null;
        }

    } // End Class ECKeyInfo 






    public class KeyConversion
    {
        private static byte[] fromRSAPublicKey(RSAPublicKey key)
        {
            DNSOutput @out = new DNSOutput();
            
            Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters kp = null; // key

            // BigInteger exponent = key.getPublicExponent();
            BigInteger exponent = kp.Exponent;

            // BigInteger modulus = key.getModulus();
            BigInteger modulus = kp.Modulus;


            int exponentLength = Helpers.BigIntegerLength(exponent);

            if (exponentLength < 256)
             {
             @out.writeU8(exponentLength);
             }
            else
            {
                @out.writeU8(0);
                @out.writeU16(exponentLength);
            }

            Helpers.writeBigInteger(@out, exponent);
            Helpers.writeBigInteger(@out, modulus);

            return @out.toByteArray();
        }


        public static byte[] fromDSAPublicKey(DSAPublicKey key)
        {
            DNSOutput @out = new DNSOutput();

            Org.BouncyCastle.Crypto.Parameters.DsaPublicKeyParameters dp = null; // Key


            // BigInteger q = key.getParams().getQ();
            BigInteger q = dp.Parameters.Q;
            // BigInteger p = key.getParams().getP();
            BigInteger p = dp.Parameters.P;
            // BigInteger g = key.getParams().getG();
            BigInteger g = dp.Parameters.G;
            // BigInteger y = key.getY();
            BigInteger y = dp.Y;


            int t = (p.ToByteArray().Length - 64) / 8;

            @out.writeU8(t);

            Helpers.writeBigInteger(@out, q);
            Helpers.writeBigInteger(@out, p);
            Helpers.writePaddedBigInteger(@out, g, 8 * t + 64);
            Helpers.writePaddedBigInteger(@out, y, 8 * t + 64);

            return @out.toByteArray();
        }


        private static byte[] fromECGOSTPublicKey(ECPublicKey key, ECKeyInfo keyinfo)
        {
            DNSOutput @out = new DNSOutput();

            Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters publicParams = null; // key

            //BigInteger x = key.getW().getAffineX();
            BigInteger x = publicParams.Q.AffineXCoord.ToBigInteger();

            //BigInteger y = key.getW().getAffineY();
            BigInteger y = publicParams.Q.AffineYCoord.ToBigInteger();


            Helpers.writePaddedBigIntegerLittleEndian(@out, x, keyinfo.length);
            Helpers.writePaddedBigIntegerLittleEndian(@out, y, keyinfo.length);

            return @out.toByteArray();
        }

        private static byte[] fromECDSAPublicKey(ECPublicKey key, ECKeyInfo keyinfo)
        {
            DNSOutput @out = new DNSOutput();

            Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters publicParams = null; // key


            // BigInteger x = key.getW().getAffineX();
            BigInteger x = publicParams.Q.AffineXCoord.ToBigInteger();

            // BigInteger y = key.getW().getAffineY();
            BigInteger y = publicParams.Q.AffineYCoord.ToBigInteger();

            Helpers.writePaddedBigInteger(@out, x, keyinfo.length);
            Helpers.writePaddedBigInteger(@out, y, keyinfo.length);

            return @out.toByteArray();
        }



        // Builds a DNSKEY record from a PublicKey
        // https://www.iana.org/assignments/dns-sec-alg-numbers/dns-sec-alg-numbers.xhtml
        public static byte[] fromPublicKey(PublicKey key, int alg) // throws DNSSECException
        {

            switch ((DnsSecAlgorithm)alg)
            {
                case DnsSecAlgorithm.RsaMd5:
                case DnsSecAlgorithm.RsaSha1:
                case DnsSecAlgorithm.RsaSha1Nsec3Sha1:
                case DnsSecAlgorithm.RsaSha256:
                case DnsSecAlgorithm.RsaSha512:
                    if (!(key is RSAPublicKey))
                    {
                        throw new IncompatibleKeyException();
                    }

                    return fromRSAPublicKey((RSAPublicKey)key);
                case DnsSecAlgorithm.Dsa:
                case DnsSecAlgorithm.DsaNsec3Sha1:
                    if (!(key is DSAPublicKey))
                    {
                        throw new IncompatibleKeyException();
                    }
                    return fromDSAPublicKey((DSAPublicKey)key);
                case DnsSecAlgorithm.EccGost:
                    if (!(key is ECPublicKey))
                    {
                        throw new IncompatibleKeyException();
                    }
                    return fromECGOSTPublicKey((ECPublicKey)key, ECKeyInfo.GOST);
                case DnsSecAlgorithm.EcDsaP256Sha256:
                    if (!(key is ECPublicKey))
                    {
                        throw new IncompatibleKeyException();
                    }
                    return fromECDSAPublicKey((ECPublicKey)key, ECKeyInfo.ECDSA_P256);
                case DnsSecAlgorithm.EcDsaP384Sha384:
                    if (!(key is ECPublicKey))
                    {
                        throw new IncompatibleKeyException();
                    }
                    return fromECDSAPublicKey((ECPublicKey)key, ECKeyInfo.ECDSA_P384);

                default:
                    throw new UnsupportedAlgorithmException(alg);
            }

        } // End Function fromPublicKey 


    } // End Class KeyConversion 


    
    internal class Helpers
    {
        internal static int BigIntegerLength(BigInteger i)
        {
            return (i.BitLength + 7) / 8;
        }


        internal static BigInteger readBigInteger(DNSInput @in, int len) // throws IOException
        {
            byte[] b = @in.readByteArray(len);
            return new BigInteger(1, b);
        }

        internal static BigInteger readBigInteger(DNSInput @in)
        {
            byte[] b = @in.readByteArray();
            return new BigInteger(1, b);
        }

        private static byte[] trimByteArray(byte[] array)
        {
            if (array[0] == 0)
            {
                byte[] trimmedArray = new byte[array.Length - 1];
                System.Array.Copy(array, 1, trimmedArray, 0, array.Length - 1);
                return trimmedArray;
            }
            else
            {
                return array;
            }
        }


        internal static void writeBigInteger(DNSOutput @out, BigInteger val)
        {
            
            byte[] b = trimByteArray(val.ToByteArray());
            @out.writeByteArray(b);
        }

        internal static void writePaddedBigInteger(DNSOutput @out, BigInteger val, int len)
        {
            byte[] b = trimByteArray(val.ToByteArray());

            if (b.Length > len)
            {
                throw new IllegalArgumentException();
            }

            if (b.Length < len)
            {
                byte[] pad = new byte[len - b.Length];
                @out.writeByteArray(pad);
            }

            @out.writeByteArray(b);
        }


        private static void reverseByteArray(byte[] array)
        {
            for (int i = 0; i < array.Length / 2; i++)
            {
                int j = array.Length - i - 1;
                byte tmp = array[i];
                array[i] = array[j];
                array[j] = tmp;
            }
        }

        internal static BigInteger readBigIntegerLittleEndian(DNSInput @in, int len) // throws IOException
        {

            byte[] b = @in.readByteArray(len);
            reverseByteArray(b);
            return new BigInteger(1, b);
        }

        internal static void writePaddedBigIntegerLittleEndian(DNSOutput @out, BigInteger val, int len)
        {
            byte[] b = trimByteArray(val.ToByteArray());

            if (b.Length > len)
            {
                throw new IllegalArgumentException();
            }

            reverseByteArray(b);
            @out.writeByteArray(b);

            if (b.Length < len)
            {
                byte[] pad = new byte[len - b.Length];
                @out.writeByteArray(pad);
            }
        }

    }


    public class KeyConversionTo
    {

        private static PublicKey toRSAPublicKey(KEYBase r) // throws IOException, GeneralSecurityException 
        {
            DNSInput @in = new DNSInput(r.getKey());

            int exponentLength = @in.readU8();
            if (exponentLength == 0)
            {
                exponentLength = @in.readU16();
            }

            BigInteger exponent = Helpers.readBigInteger(@in, exponentLength);
            BigInteger modulus = Helpers.readBigInteger(@in);
            /*
            KeyFactory factory = KeyFactory.getInstance("RSA");
            return factory.generatePublic(new RSAPublicKeySpec(modulus, exponent));
            */

            

            Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters kp
                = new Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters(false, modulus, exponent);

            return null;
        }


        private static PublicKey toDSAPublicKey(KEYBase r) // throws IOException, GeneralSecurityException, MalformedKeyException 
        {
            DNSInput @in = new DNSInput(r.getKey());

            int t = @in.readU8();
            if (t > 8)
            {
                throw new MalformedKeyException(r);
            }

            BigInteger q = Helpers.readBigInteger(@in, 20);
            BigInteger p = Helpers.readBigInteger(@in, 64 + t * 8);
            BigInteger g = Helpers.readBigInteger(@in, 64 + t * 8);
            BigInteger y = Helpers.readBigInteger(@in, 64 + t * 8);
            /*
            KeyFactory factory = KeyFactory.getInstance("DSA");
            return factory.generatePublic(new DSAPublicKeySpec(y, p, q, g));
            */

            Org.BouncyCastle.Crypto.Parameters.DsaParameters para = new Org.BouncyCastle.Crypto.Parameters.DsaParameters(p, q, g);

            Org.BouncyCastle.Crypto.Parameters.DsaPublicKeyParameters dp = new Org.BouncyCastle.Crypto.Parameters.DsaPublicKeyParameters(y, para);

            return null;
        }


        /*
         // https://csharp.hotexamples.com/examples/Org.BouncyCastle.Crypto.Parameters/ECPrivateKeyParameters/-/php-ecprivatekeyparameters-class-examples.html
         public ECDiffieHellmanBc(Int32 keySize)
        {
            Org.BouncyCastle.Asn1.X9.X9ECParameters ecParams;
            switch (keySize) {
            case 256:
                ecParams = Org.BouncyCastle.Asn1.Sec.SecNamedCurves.GetByName("secp256r1");
                break;
            case 384:
                ecParams = Org.BouncyCastle.Asn1.Sec.SecNamedCurves.GetByName("secp384r1");
                break;
            case 521:
                ecParams = Org.BouncyCastle.Asn1.Sec.SecNamedCurves.GetByName("secp521r1");
                break;
            default:
        */


        // https://www.iana.org/assignments/dns-sec-alg-numbers/dns-sec-alg-numbers.xhtml
        // https://tools.ietf.org/html/rfc5933
        private static PublicKey toECGOSTPublicKey(KEYBase r, ECKeyInfo keyinfo) // throws IOException, GeneralSecurityException 
        {
            DNSInput @in = new DNSInput(r.getKey());

            BigInteger x = Helpers.readBigIntegerLittleEndian(@in, keyinfo.length);
            BigInteger y = Helpers.readBigIntegerLittleEndian(@in, keyinfo.length);
            
            // OID to be found in Org.BouncyCastle.Security.GeneratorUtilities.GetKeyPairGenerator("ECGOST3410");
            Org.BouncyCastle.Crypto.Parameters.ECDomainParameters domain = Org.BouncyCastle.Asn1.CryptoPro.ECGost3410NamedCurves.GetByOid(Org.BouncyCastle.Asn1.CryptoPro.CryptoProObjectIdentifiers.GostR3410x94CryptoProA);
            Org.BouncyCastle.Math.EC.ECCurve c = domain.Curve;
            Org.BouncyCastle.Math.EC.ECPoint q = new Org.BouncyCastle.Math.EC.FpPoint(c, c.FromBigInteger(x), c.FromBigInteger(y));

            Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters publicParams = new Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters(q, domain);



            // Org.BouncyCastle.Crypto.Signers.ECGost3410Signer

            // Org.BouncyCastle.Security.PublicKeyFactory.CreateKey(new Org.BouncyCastle.Asn1.X509.SubjectPublicKeyInfo())

            /*
            ECPoint q = new ECPoint(x, y);

            KeyFactory factory = KeyFactory.getInstance("ECGOST3410");
            return factory.generatePublic(new ECPublicKeySpec(q, keyinfo.spec));
            */

            return null;
        }



        // https://stackoverflow.com/questions/17439732/recreating-keys-ecpublickeyparameters-in-c-sharp-with-bouncycastle
        // TODO: find curve name...
        private static Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters CreateEcPublicKeyParameters(BigInteger xx, BigInteger yy)
        {
            // Org.BouncyCastle.Math.EC.ECPoint q = new Org.BouncyCastle.Math.EC.ECPoint(x, y);
            // Org.BouncyCastle.Crypto.Tls.NamedCurve.secp224k1


            // DefineCurveAlias("P-256", SecObjectIdentifiers.SecP256r1); // Alg 13
            // DefineCurveAlias("P-384", SecObjectIdentifiers.SecP384r1); // Alg 14
            // DefineCurveAlias("P-521", SecObjectIdentifiers.SecP521r1);

            string curveName = "P-521";
            Org.BouncyCastle.Asn1.X9.X9ECParameters ecP = Org.BouncyCastle.Asn1.Nist.NistNamedCurves.GetByName(curveName);
            Org.BouncyCastle.Math.EC.FpCurve c = (Org.BouncyCastle.Math.EC.FpCurve)ecP.Curve;
            
            Org.BouncyCastle.Math.EC.ECFieldElement x = c.FromBigInteger(xx);
            Org.BouncyCastle.Math.EC.ECFieldElement y = c.FromBigInteger(yy);
            Org.BouncyCastle.Math.EC.ECPoint q = new Org.BouncyCastle.Math.EC.FpPoint(c, x, y);


            Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters publicParams = null;
            // Org.BouncyCastle.Crypto.Parameters.ECKeyParameters publicParams = new Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters(q, domain);
            // Org.BouncyCastle.Crypto.AsymmetricKeyParameter publicParams = new Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters(q, domain);
            // Org.BouncyCastle.Crypto.ICipherParameters publicParams = new Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters(q, domain);


            // Org.BouncyCastle.Crypto.Digests.GeneralDigest.
            // Org.BouncyCastle.Crypto.Signers.GenericSigner
            // Org.BouncyCastle.Crypto.Generators.ECKeyPairGenerator


            // Org.BouncyCastle.Security.SignerUtilities.GetSigner("SHA-384withRSA");
            // Org.BouncyCastle.Security.DigestUtilities.GetDigest("");
            // Org.BouncyCastle.Security.CipherUtilities.GetCipher("");
            // Org.BouncyCastle.Security.GeneratorUtilities.GetKeyGenerator("");
            // Org.BouncyCastle.Security.WrapperUtilities.GetAlgorithmName
            // Org.BouncyCastle.Security.MacUtilities.CalculateMac("", null, System.Text.Encoding.UTF8.GetBytes("HashThis"));
            // Org.BouncyCastle.Security.ParameterUtilities.CreateKeyParameter("name", new byte[] { });
            // Org.BouncyCastle.Security.ParameterUtilities.GenerateParameters("name", new Org.BouncyCastle.Security.SecureRandom());
            // Org.BouncyCastle.Security.PublicKeyFactory.CreateKey()



            // Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters oara = new Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters("ECDH", q, Org.BouncyCastle.Asn1.Sec.SecObjectIdentifiers.SecP521r1);

            return publicParams;
        }


        private static PublicKey toECDSAPublicKey(KEYBase r, ECKeyInfo keyinfo) // throws IOException, GeneralSecurityException 
        {
            DNSInput @in = new DNSInput(r.getKey());

            // RFC 6605 Section 4
            BigInteger x = Helpers.readBigInteger(@in, keyinfo.length);
            BigInteger y = Helpers.readBigInteger(@in, keyinfo.length);




            // Org.BouncyCastle.Asn1.X9.X9ECParameters ecParams = Org.BouncyCastle.Asn1.Sec.SecNamedCurves.GetByName("secp521r1");
            // Org.BouncyCastle.Asn1.X9.X9ECParameters ecParams = Org.BouncyCastle.Asn1.Sec.SecNamedCurves.GetByName("secp256r1");
            // Org.BouncyCastle.Asn1.X9.X9ECParameters p = Org.BouncyCastle.Asn1.X9.X962NamedCurves.GetByOid(Org.BouncyCastle.Asn1.X9.X9ObjectIdentifiers.Prime239v1);
            // Org.BouncyCastle.Asn1.X9.X9ECParameters p = Org.BouncyCastle.Asn1.X9.X962NamedCurves.GetByOid(Org.BouncyCastle.Asn1.X9.X9ObjectIdentifiers.ECDsaWithSha512);
            // Org.BouncyCastle.Asn1.X9.X9ECParameters ecP = Org.BouncyCastle.Asn1.Nist.NistNamedCurves.GetByName("curveName");
            // Org.BouncyCastle.Math.EC.FpCurve c = (Org.BouncyCastle.Math.EC.FpCurve)ecP.Curve;


            Org.BouncyCastle.Asn1.X9.X9ECParameters p = Org.BouncyCastle.Asn1.X9.X962NamedCurves.GetByOid(Org.BouncyCastle.Asn1.X9.X9ObjectIdentifiers.ECDsaWithSha512);
            Org.BouncyCastle.Math.EC.ECCurve c = p.Curve;
            // Org.BouncyCastle.Crypto.Parameters.ECDomainParameters domain = new Org.BouncyCastle.Crypto.Parameters.ECDomainParameters(c, p.G, p.N, p.H);
            // Org.BouncyCastle.Math.EC.ECPoint q = new Org.BouncyCastle.Math.EC.FpPoint(c, c.FromBigInteger(x), c.FromBigInteger(y));
            // Org.BouncyCastle.Math.EC.ECPoint q = c.CreatePoint(x, y);
            // Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters publicParams = new Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters(q, domain);


            Org.BouncyCastle.Math.EC.ECPoint q = keyinfo.curve.CreatePoint(x, y);
            Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters publicParams = new Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters(q, keyinfo.spec);



            // Org.BouncyCastle.Security.PublicKeyFactory.CreateKey(Org.BouncyCastle.X509.SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicParams));

            // Org.BouncyCastle.Security.PrivateKeyFactory.CreateKey(publicParams);
            // Org.BouncyCastle.Security.PrivateKeyFactory
            // Org.BouncyCastle.Security.PublicKeyFactory
            // Org.BouncyCastle.Security.PublicKeyFactory.CreateKey(new Org.BouncyCastle.Asn1.X509.SubjectPublicKeyInfo())

            /*
            ECPoint q = new ECPoint(x, y);

            KeyFactory factory = KeyFactory.getInstance("EC");
            return factory.generatePublic(new ECPublicKeySpec(q, keyinfo.spec));
            */

            return null;
        }


        // Converts a KEY/DNSKEY record into a PublicKey 
        // https://www.iana.org/assignments/dns-sec-alg-numbers/dns-sec-alg-numbers.xhtml
        static PublicKey toPublicKey(KEYBase r) // throws DNSSECException
        {
            int alg = r.getAlgorithm();
            try
            {
                switch ((DnsSecAlgorithm)alg)
                {
                    case DnsSecAlgorithm.RsaMd5:
                    case DnsSecAlgorithm.RsaSha1:
                    case DnsSecAlgorithm.RsaSha1Nsec3Sha1:
                    case DnsSecAlgorithm.RsaSha256:
                    case DnsSecAlgorithm.RsaSha512:
                        return toRSAPublicKey(r);
                    case DnsSecAlgorithm.Dsa:
                    case DnsSecAlgorithm.DsaNsec3Sha1:
                        return toDSAPublicKey(r);
                    case DnsSecAlgorithm.EccGost:
                        return toECGOSTPublicKey(r, ECKeyInfo.GOST);
                    case DnsSecAlgorithm.EcDsaP256Sha256:
                        return toECDSAPublicKey(r, ECKeyInfo.ECDSA_P256);
                    case DnsSecAlgorithm.EcDsaP384Sha384:
                        return toECDSAPublicKey(r, ECKeyInfo.ECDSA_P384);
                    default:
                        throw new UnsupportedAlgorithmException(alg);
                }
            }
            catch (System.IO.IOException e)
            {
                throw new MalformedKeyException(r, e);
            }
            catch (Org.BouncyCastle.Security.GeneralSecurityException e)
            {
                throw new DNSSECException(e);
            }
        } // End Function toPublicKey 


    } // End Class KeyConversionTo 



} // End Namespace 
