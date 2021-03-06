﻿
namespace ArsoftTestServer
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
        : System.Exception
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


        // int src1, src2, ans;
        // ans = src1 >>> src2;
        // https://stackoverflow.com/questions/19058859/what-does-mean-in-java/19058871
        private int rightMove(int value, int pos)
        {
            if (pos != 0)
            {
                int mask = 0x7fffffff;
                value >>= 1;
                value &= mask;
                value >>= pos - 1;
            }
            return value;
        }


        // Writes an unsigned 16 bit value to the stream.
        // @param val The value to be written
        public void writeU16(int val)
        {
            check(val, 16);
            need(2);

            // The >>> operator is the unsigned right bit-shift operator in Java. 
            // It effectively divides the operand by 2 to the power of the right operand, or just 2 here.
            // The difference between >> and >>> would only show up when shifting negative numbers. 
            // The >> operator shifts a 1 bit into the most significant bit if it was a 1, and the >>> shifts in a 0 regardless.

            // array[pos++] = (byte)((val >>> 8) & 0xFF);
            // array[pos++] = (byte)(rightMove(val, 8) & 0xFF);

            int uval = (int)((uint)val >> 8);
            array[pos++] = (byte)(uval & 0xFF);
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


    public abstract class PublicKey
    {

        public abstract Org.BouncyCastle.Crypto.AsymmetricKeyParameter Public_Key { get; }


        public static PublicKey CreateInstance(Org.BouncyCastle.Crypto.AsymmetricKeyParameter publicKey)
        {
            if (publicKey is Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters)
                return new ECPublicKey((Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters)publicKey);

            if (publicKey is Org.BouncyCastle.Crypto.Parameters.DsaPublicKeyParameters)
                return new DSAPublicKey((Org.BouncyCastle.Crypto.Parameters.DsaPublicKeyParameters)publicKey);

            if (publicKey is Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters)
                return new RSAPublicKey((Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters)publicKey);

            throw new System.NotSupportedException("Unrecognized AsymmetricKeyParameter");
        }

    }


    public class RSAPublicKey
        : PublicKey
    {

        protected Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters m_publicKey;

        public override Org.BouncyCastle.Crypto.AsymmetricKeyParameter Public_Key { get { return this.m_publicKey; } }


        public Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters PublicKey { get { return this.m_publicKey; } }


        public RSAPublicKey(Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters publicKey)
        {
            m_publicKey = publicKey;
        }

    }

    public class DSAPublicKey
        : PublicKey
    {
        protected Org.BouncyCastle.Crypto.Parameters.DsaPublicKeyParameters m_publicKey;

        public override Org.BouncyCastle.Crypto.AsymmetricKeyParameter Public_Key { get { return this.m_publicKey; } }

        public Org.BouncyCastle.Crypto.Parameters.DsaPublicKeyParameters PublicKey { get { return this.m_publicKey; } }


        public DSAPublicKey(Org.BouncyCastle.Crypto.Parameters.DsaPublicKeyParameters publicKey)
        {
            m_publicKey = publicKey;
        }
    }

    public class ECPublicKey
        : PublicKey
    {
        protected Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters m_publicKey;

        public override Org.BouncyCastle.Crypto.AsymmetricKeyParameter Public_Key { get { return this.m_publicKey; } }
        public Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters PublicKey { get { return this.m_publicKey; } }

        public ECPublicKey(Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters publicKey)
        {
            m_publicKey = publicKey;
        }
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



    public class KEYBase
    {

        protected byte[] m_key;
        protected int m_algorithm;


        public KEYBase()
        { }


        public KEYBase(byte[] key, ARSoft.Tools.Net.Dns.DnsSecAlgorithm algorithm)
        {
            this.m_key = key;
            this.m_algorithm = (int)algorithm;
        }



        // Returns the binary data representing the key
        public byte[] getKey()
        {
            return this.m_key;
        }

        public int getAlgorithm()
        {
            return this.m_algorithm;
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


        private static Org.BouncyCastle.Math.BigInteger FromHex(string hex)
        {
            return new Org.BouncyCastle.Math.BigInteger(1, Org.BouncyCastle.Utilities.Encoders.Hex.Decode(hex));
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
            p = new Org.BouncyCastle.Math.BigInteger(p_str, 16);
            a = new Org.BouncyCastle.Math.BigInteger(a_str, 16);
            b = new Org.BouncyCastle.Math.BigInteger(b_str, 16);
            gx = new Org.BouncyCastle.Math.BigInteger(gx_str, 16);
            gy = new Org.BouncyCastle.Math.BigInteger(gy_str, 16);
            n = new Org.BouncyCastle.Math.BigInteger(n_str, 16);
            Org.BouncyCastle.Math.BigInteger h = Org.BouncyCastle.Math.BigInteger.One;

            this.curve = new Org.BouncyCastle.Math.EC.FpCurve(p, a, b, n, h);

            //spec = new ECParameterSpec(curve, new ECPoint(gx, gy), n, 1);
            this.spec = new Org.BouncyCastle.Crypto.Parameters.ECDomainParameters(
                 curve
                , curve.CreatePoint(gx, gy) // G 
                , n
                , h
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


    } // End Class ECKeyInfo 






    public class KeyConversion
    {
        private static byte[] fromRSAPublicKey(RSAPublicKey key)
        {
            DNSOutput @out = new DNSOutput();

            Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters kp = key.PublicKey;

            // BigInteger exponent = key.getPublicExponent();
            Org.BouncyCastle.Math.BigInteger exponent = kp.Exponent;

            // BigInteger modulus = key.getModulus();
            Org.BouncyCastle.Math.BigInteger modulus = kp.Modulus;


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

            Org.BouncyCastle.Crypto.Parameters.DsaPublicKeyParameters dp = key.PublicKey;


            // BigInteger q = key.getParams().getQ();
            Org.BouncyCastle.Math.BigInteger q = dp.Parameters.Q;
            // BigInteger p = key.getParams().getP();
            Org.BouncyCastle.Math.BigInteger p = dp.Parameters.P;
            // BigInteger g = key.getParams().getG();
            Org.BouncyCastle.Math.BigInteger g = dp.Parameters.G;
            // BigInteger y = key.getY();
            Org.BouncyCastle.Math.BigInteger y = dp.Y;


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

            Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters publicParams = key.PublicKey;

            //BigInteger x = key.getW().getAffineX();
            Org.BouncyCastle.Math.BigInteger x = publicParams.Q.AffineXCoord.ToBigInteger();

            //BigInteger y = key.getW().getAffineY();
            Org.BouncyCastle.Math.BigInteger y = publicParams.Q.AffineYCoord.ToBigInteger();


            Helpers.writePaddedBigIntegerLittleEndian(@out, x, keyinfo.length);
            Helpers.writePaddedBigIntegerLittleEndian(@out, y, keyinfo.length);

            return @out.toByteArray();
        }


        private static byte[] fromECDSAPublicKey(ECPublicKey key, ECKeyInfo keyinfo)
        {
            DNSOutput @out = new DNSOutput();

            Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters publicParams = key.PublicKey;


            // BigInteger x = key.getW().getAffineX();
            Org.BouncyCastle.Math.BigInteger x = publicParams.Q.AffineXCoord.ToBigInteger();

            // BigInteger y = key.getW().getAffineY();
            Org.BouncyCastle.Math.BigInteger y = publicParams.Q.AffineYCoord.ToBigInteger();

            Helpers.writePaddedBigInteger(@out, x, keyinfo.length);
            Helpers.writePaddedBigInteger(@out, y, keyinfo.length);

            return @out.toByteArray();
        }


        // Builds a DNSKEY record from a PublicKey
        // https://www.iana.org/assignments/dns-sec-alg-numbers/dns-sec-alg-numbers.xhtml
        public static byte[] fromPublicKey(PublicKey key, int alg) // throws DNSSECException
        {

            switch ((ARSoft.Tools.Net.Dns.DnsSecAlgorithm)alg)
            {
                case ARSoft.Tools.Net.Dns.DnsSecAlgorithm.RsaMd5:
                case ARSoft.Tools.Net.Dns.DnsSecAlgorithm.RsaSha1:
                case ARSoft.Tools.Net.Dns.DnsSecAlgorithm.RsaSha1Nsec3Sha1:
                case ARSoft.Tools.Net.Dns.DnsSecAlgorithm.RsaSha256:
                case ARSoft.Tools.Net.Dns.DnsSecAlgorithm.RsaSha512:
                    if (!(key is RSAPublicKey))
                    {
                        throw new IncompatibleKeyException();
                    }

                    return fromRSAPublicKey((RSAPublicKey)key);
                case ARSoft.Tools.Net.Dns.DnsSecAlgorithm.Dsa:
                case ARSoft.Tools.Net.Dns.DnsSecAlgorithm.DsaNsec3Sha1:
                    if (!(key is DSAPublicKey))
                    {
                        throw new IncompatibleKeyException();
                    }
                    return fromDSAPublicKey((DSAPublicKey)key);
                case ARSoft.Tools.Net.Dns.DnsSecAlgorithm.EccGost:
                    if (!(key is ECPublicKey))
                    {
                        throw new IncompatibleKeyException();
                    }
                    return fromECGOSTPublicKey((ECPublicKey)key, ECKeyInfo.GOST);
                case ARSoft.Tools.Net.Dns.DnsSecAlgorithm.EcDsaP256Sha256:
                    if (!(key is ECPublicKey))
                    {
                        throw new IncompatibleKeyException();
                    }
                    return fromECDSAPublicKey((ECPublicKey)key, ECKeyInfo.ECDSA_P256);
                case ARSoft.Tools.Net.Dns.DnsSecAlgorithm.EcDsaP384Sha384:
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
        internal static int BigIntegerLength(Org.BouncyCastle.Math.BigInteger i)
        {
            return (i.BitLength + 7) / 8;
        }


        internal static Org.BouncyCastle.Math.BigInteger readBigInteger(DNSInput @in, int len) // throws IOException
        {
            byte[] b = @in.readByteArray(len);
            return new Org.BouncyCastle.Math.BigInteger(1, b);
        }

        internal static Org.BouncyCastle.Math.BigInteger readBigInteger(DNSInput @in)
        {
            byte[] b = @in.readByteArray();
            return new Org.BouncyCastle.Math.BigInteger(1, b);
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


        internal static void writeBigInteger(DNSOutput @out, Org.BouncyCastle.Math.BigInteger val)
        {

            byte[] b = trimByteArray(val.ToByteArray());
            @out.writeByteArray(b);
        }

        internal static void writePaddedBigInteger(DNSOutput @out, Org.BouncyCastle.Math.BigInteger val, int len)
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

        internal static Org.BouncyCastle.Math.BigInteger readBigIntegerLittleEndian(DNSInput @in, int len) // throws IOException
        {

            byte[] b = @in.readByteArray(len);
            reverseByteArray(b);
            return new Org.BouncyCastle.Math.BigInteger(1, b);
        }

        internal static void writePaddedBigIntegerLittleEndian(DNSOutput @out, Org.BouncyCastle.Math.BigInteger val, int len)
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

            Org.BouncyCastle.Math.BigInteger exponent = Helpers.readBigInteger(@in, exponentLength);
            Org.BouncyCastle.Math.BigInteger modulus = Helpers.readBigInteger(@in);
            /*
            KeyFactory factory = KeyFactory.getInstance("RSA");
            return factory.generatePublic(new RSAPublicKeySpec(modulus, exponent));
            */

            Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters kp
                = new Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters(false, modulus, exponent);

            return PublicKey.CreateInstance(kp);
        }


        private static PublicKey toDSAPublicKey(KEYBase r) // throws IOException, GeneralSecurityException, MalformedKeyException 
        {
            DNSInput @in = new DNSInput(r.getKey());

            int t = @in.readU8();
            if (t > 8)
            {
                throw new MalformedKeyException(r);
            }

            Org.BouncyCastle.Math.BigInteger q = Helpers.readBigInteger(@in, 20);
            Org.BouncyCastle.Math.BigInteger p = Helpers.readBigInteger(@in, 64 + t * 8);
            Org.BouncyCastle.Math.BigInteger g = Helpers.readBigInteger(@in, 64 + t * 8);
            Org.BouncyCastle.Math.BigInteger y = Helpers.readBigInteger(@in, 64 + t * 8);
            /*
            KeyFactory factory = KeyFactory.getInstance("DSA");
            return factory.generatePublic(new DSAPublicKeySpec(y, p, q, g));
            */

            Org.BouncyCastle.Crypto.Parameters.DsaParameters para = new Org.BouncyCastle.Crypto.Parameters.DsaParameters(p, q, g);

            Org.BouncyCastle.Crypto.Parameters.DsaPublicKeyParameters dp = new Org.BouncyCastle.Crypto.Parameters.DsaPublicKeyParameters(y, para);

            return PublicKey.CreateInstance(dp);
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

            Org.BouncyCastle.Math.BigInteger x = Helpers.readBigIntegerLittleEndian(@in, keyinfo.length);
            Org.BouncyCastle.Math.BigInteger y = Helpers.readBigIntegerLittleEndian(@in, keyinfo.length);

            // OID to be found in Org.BouncyCastle.Security.GeneratorUtilities.GetKeyPairGenerator("ECGOST3410");
            // Org.BouncyCastle.Crypto.Parameters.ECDomainParameters domain = Org.BouncyCastle.Asn1.CryptoPro.ECGost3410NamedCurves.GetByOid(Org.BouncyCastle.Asn1.CryptoPro.CryptoProObjectIdentifiers.GostR3410x94CryptoProA);
            // Org.BouncyCastle.Math.EC.ECCurve c = domain.Curve;
            // Org.BouncyCastle.Math.EC.ECPoint q = new Org.BouncyCastle.Math.EC.FpPoint(c, c.FromBigInteger(x), c.FromBigInteger(y));

            // Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters publicParams = new Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters(q, domain);



            Org.BouncyCastle.Math.EC.ECPoint q = keyinfo.curve.CreatePoint(x, y);
            Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters publicParams = new Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters(q, keyinfo.spec);


            // Org.BouncyCastle.Crypto.Signers.ECGost3410Signer

            // Org.BouncyCastle.Security.PublicKeyFactory.CreateKey(new Org.BouncyCastle.Asn1.X509.SubjectPublicKeyInfo())

            /*
            ECPoint q = new ECPoint(x, y);

            KeyFactory factory = KeyFactory.getInstance("ECGOST3410");
            return factory.generatePublic(new ECPublicKeySpec(q, keyinfo.spec));
            */

            return PublicKey.CreateInstance(publicParams);
        }



        // https://stackoverflow.com/questions/17439732/recreating-keys-ecpublickeyparameters-in-c-sharp-with-bouncycastle
        // TODO: find curve name...
        private static Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters CreateEcPublicKeyParameters(
            Org.BouncyCastle.Math.BigInteger xx
            , Org.BouncyCastle.Math.BigInteger yy)
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
            Org.BouncyCastle.Math.BigInteger x = Helpers.readBigInteger(@in, keyinfo.length);
            Org.BouncyCastle.Math.BigInteger y = Helpers.readBigInteger(@in, keyinfo.length);


            // Org.BouncyCastle.Asn1.X9.X9ECParameters ecParams = Org.BouncyCastle.Asn1.Sec.SecNamedCurves.GetByName("secp521r1");
            // Org.BouncyCastle.Asn1.X9.X9ECParameters ecParams = Org.BouncyCastle.Asn1.Sec.SecNamedCurves.GetByName("secp256r1");
            // Org.BouncyCastle.Asn1.X9.X9ECParameters p = Org.BouncyCastle.Asn1.X9.X962NamedCurves.GetByOid(Org.BouncyCastle.Asn1.X9.X9ObjectIdentifiers.Prime239v1);
            // Org.BouncyCastle.Asn1.X9.X9ECParameters p = Org.BouncyCastle.Asn1.X9.X962NamedCurves.GetByOid(Org.BouncyCastle.Asn1.X9.X9ObjectIdentifiers.ECDsaWithSha512);
            // Org.BouncyCastle.Asn1.X9.X9ECParameters ecP = Org.BouncyCastle.Asn1.Nist.NistNamedCurves.GetByName("curveName");
            // Org.BouncyCastle.Math.EC.FpCurve c = (Org.BouncyCastle.Math.EC.FpCurve)ecP.Curve;


            // Org.BouncyCastle.Asn1.X9.X9ECParameters p = Org.BouncyCastle.Asn1.X9.X962NamedCurves.GetByOid(Org.BouncyCastle.Asn1.X9.X9ObjectIdentifiers.ECDsaWithSha512);
            // Org.BouncyCastle.Math.EC.ECCurve c = p.Curve;
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

            return PublicKey.CreateInstance(publicParams);
        }


        // Converts a KEY/DNSKEY record into a PublicKey 
        // https://www.iana.org/assignments/dns-sec-alg-numbers/dns-sec-alg-numbers.xhtml
        public static PublicKey toPublicKey(KEYBase r) // throws DNSSECException
        {
            int alg = r.getAlgorithm();
            try
            {
                switch ((ARSoft.Tools.Net.Dns.DnsSecAlgorithm)alg)
                {
                    case ARSoft.Tools.Net.Dns.DnsSecAlgorithm.RsaMd5:
                    case ARSoft.Tools.Net.Dns.DnsSecAlgorithm.RsaSha1:
                    case ARSoft.Tools.Net.Dns.DnsSecAlgorithm.RsaSha1Nsec3Sha1:
                    case ARSoft.Tools.Net.Dns.DnsSecAlgorithm.RsaSha256:
                    case ARSoft.Tools.Net.Dns.DnsSecAlgorithm.RsaSha512:
                        return toRSAPublicKey(r);
                    case ARSoft.Tools.Net.Dns.DnsSecAlgorithm.Dsa:
                    case ARSoft.Tools.Net.Dns.DnsSecAlgorithm.DsaNsec3Sha1:
                        return toDSAPublicKey(r);
                    case ARSoft.Tools.Net.Dns.DnsSecAlgorithm.EccGost:
                        return toECGOSTPublicKey(r, ECKeyInfo.GOST);
                    case ARSoft.Tools.Net.Dns.DnsSecAlgorithm.EcDsaP256Sha256:
                        return toECDSAPublicKey(r, ECKeyInfo.ECDSA_P256);
                    case ARSoft.Tools.Net.Dns.DnsSecAlgorithm.EcDsaP384Sha384:
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
