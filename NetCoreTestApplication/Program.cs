
using System.Collections.Generic;
using ARSoft.Tools.Net.Dns;
using ArsoftTestServer;

namespace NetCoreTestApplication
{
    using ARSoft.Tools.Net;
    using ARSoft.Tools.Net.Dns;

    class Program
    {


        public static byte[] Base64ToByteArray(string base64encoded)
        {
            base64encoded = System.Text.RegularExpressions.Regex.Replace(base64encoded, @"\s+", "");
            byte[] bytes = System.Convert.FromBase64String(base64encoded);
            return bytes;
        }


        public static Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair GenerateRsaKeyPair(Org.BouncyCastle.Security.SecureRandom random, int strength)
        {
            Org.BouncyCastle.Crypto.Generators.RsaKeyPairGenerator gen = new Org.BouncyCastle.Crypto.Generators.RsaKeyPairGenerator();

            Org.BouncyCastle.Crypto.KeyGenerationParameters keyGenParam =
                new Org.BouncyCastle.Crypto.KeyGenerationParameters(random, strength);

            gen.Init(keyGenParam);

            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair kp = gen.GenerateKeyPair();
            return kp;
        } // End Sub GenerateRsaKeyPair 


        public static Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair GenerateEcdsaKeyPair(Org.BouncyCastle.Security.SecureRandom random)
        {
            Org.BouncyCastle.Crypto.Generators.ECKeyPairGenerator gen =
                new Org.BouncyCastle.Crypto.Generators.ECKeyPairGenerator();

            // https://github.com/bcgit/bc-csharp/blob/master/crypto/src/asn1/sec/SECNamedCurves.cs#LC1096
            Org.BouncyCastle.Asn1.X9.X9ECParameters ps =
                Org.BouncyCastle.Asn1.Sec.SecNamedCurves.GetByName("secp256k1");

            Org.BouncyCastle.Crypto.Parameters.ECDomainParameters ecParams =
                new Org.BouncyCastle.Crypto.Parameters.ECDomainParameters(ps.Curve, ps.G, ps.N, ps.H);

            Org.BouncyCastle.Crypto.Parameters.ECKeyGenerationParameters keyGenParam =
                new Org.BouncyCastle.Crypto.Parameters.ECKeyGenerationParameters(ecParams, random);

            gen.Init(keyGenParam);
            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair kp = gen.GenerateKeyPair();

            return kp;
        } // End Function GenerateEcdsaKeyPair 


        public static Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair
            GenerateDsaKeyPair(Org.BouncyCastle.Security.SecureRandom random, int keystrength)
        {
            Org.BouncyCastle.Crypto.IAsymmetricCipherKeyPairGenerator keypairGen = new Org.BouncyCastle.Crypto.Generators.DsaKeyPairGenerator();

            Org.BouncyCastle.Crypto.Generators.DsaParametersGenerator pGen = new Org.BouncyCastle.Crypto.Generators.DsaParametersGenerator();
            pGen.Init(keystrength, 80, random); //TODO:
            Org.BouncyCastle.Crypto.Parameters.DsaParameters parameters = pGen.GenerateParameters();
            Org.BouncyCastle.Crypto.Parameters.DsaKeyGenerationParameters genParam = new Org.BouncyCastle.Crypto.Parameters.DsaKeyGenerationParameters(random, parameters);
            keypairGen.Init(genParam);

            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair kp = keypairGen.GenerateKeyPair();
            return kp;
        }


        // https://stackoverflow.com/questions/33813108/bouncycastle-diffie-hellman
        public static Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair GenerateDiffieHellmanKeyPair(Org.BouncyCastle.Security.SecureRandom random
            , int keystrength
            )
        {
            const int DefaultPrimeProbability = 30;

            Org.BouncyCastle.Crypto.IAsymmetricCipherKeyPairGenerator keypairGen = new Org.BouncyCastle.Crypto.Generators.DHKeyPairGenerator();

            Org.BouncyCastle.Crypto.Generators.DHParametersGenerator pGen = new Org.BouncyCastle.Crypto.Generators.DHParametersGenerator();
            pGen.Init(keystrength, DefaultPrimeProbability, random);

            Org.BouncyCastle.Crypto.Parameters.DHParameters parameters = pGen.GenerateParameters();
            Org.BouncyCastle.Crypto.KeyGenerationParameters genParam = new Org.BouncyCastle.Crypto.Parameters.DHKeyGenerationParameters(new Org.BouncyCastle.Security.SecureRandom(), parameters);
            keypairGen.Init(genParam);

            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair kp = keypairGen.GenerateKeyPair();
            return kp;
        }


        public static Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair
            GenerateBadGost3410KeyPair(Org.BouncyCastle.Security.SecureRandom random, int keystrength)
        {
            // Org.BouncyCastle.Crypto.IAsymmetricCipherKeyPairGenerator keypairGen1 = Org.BouncyCastle.Security.GeneratorUtilities.GetKeyPairGenerator("DSA");


            // NID_id_GostR3410_2001_CryptoPro_A_ParamSet
            Org.BouncyCastle.Crypto.IAsymmetricCipherKeyPairGenerator keypairGen = new Org.BouncyCastle.Crypto.Generators.Gost3410KeyPairGenerator();

            Org.BouncyCastle.Crypto.Generators.Gost3410ParametersGenerator pGen = new Org.BouncyCastle.Crypto.Generators.Gost3410ParametersGenerator();
            pGen.Init(keystrength, 2, random); //TODO:

            Org.BouncyCastle.Crypto.Parameters.Gost3410KeyGenerationParameters kgp = new Org.BouncyCastle.Crypto.Parameters.Gost3410KeyGenerationParameters(
                random
            // , Org.BouncyCastle.Asn1.CryptoPro.CryptoProObjectIdentifiers.GostR3410x94CryptoProA
            // , Org.BouncyCastle.Asn1.CryptoPro.CryptoProObjectIdentifiers.GostR3410x2001CryptoProA
            // , Org.BouncyCastle.Asn1.CryptoPro.CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x94
            // , Org.BouncyCastle.Asn1.CryptoPro.CryptoProObjectIdentifiers.GostR3411x94CryptoProParamSet
            , Org.BouncyCastle.Asn1.CryptoPro.CryptoProObjectIdentifiers.GostR3410x2001
            );

            Org.BouncyCastle.Crypto.Parameters.Gost3410Parameters parameters = pGen.GenerateParameters();
            Org.BouncyCastle.Crypto.Parameters.Gost3410KeyGenerationParameters genParam = new Org.BouncyCastle.Crypto.Parameters.Gost3410KeyGenerationParameters(random, parameters);
            keypairGen.Init(genParam);

            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair kp = keypairGen.GenerateKeyPair();
            return kp;
        }


        public static Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair
            GenerateGost3410KeyPair(Org.BouncyCastle.Security.SecureRandom random, int keystrength)
        {
            Org.BouncyCastle.Crypto.Parameters.ECDomainParameters gostEcDomainParameters =
                Org.BouncyCastle.Asn1.CryptoPro.ECGost3410NamedCurves.GetByOid(
                    Org.BouncyCastle.Asn1.CryptoPro.CryptoProObjectIdentifiers.GostR3410x2001CryptoProA);

            Org.BouncyCastle.Crypto.IAsymmetricCipherKeyPairGenerator gostKeyGen = new Org.BouncyCastle.Crypto.Generators.ECKeyPairGenerator();
            Org.BouncyCastle.Crypto.KeyGenerationParameters genParam = new Org.BouncyCastle.Crypto.Parameters.ECKeyGenerationParameters(gostEcDomainParameters, random);
            gostKeyGen.Init(genParam);

            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair kp = gostKeyGen.GenerateKeyPair();
            return kp;
        }


        public static void GenerateKeypair(DnsSecAlgorithm algo, out byte[] privateKey, out byte[] publicKey)
        {
            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair keyPair = null;

            Org.BouncyCastle.Security.SecureRandom random =
                new Org.BouncyCastle.Security.SecureRandom(new Org.BouncyCastle.Crypto.Prng.CryptoApiRandomGenerator());

            switch (algo)
            {
                case DnsSecAlgorithm.RsaMd5:
                case DnsSecAlgorithm.RsaSha1:
                case DnsSecAlgorithm.RsaSha256:
                case DnsSecAlgorithm.RsaSha512:
                case DnsSecAlgorithm.RsaSha1Nsec3Sha1:
                    keyPair = GenerateRsaKeyPair(random, 4096);
                    break;
                case DnsSecAlgorithm.DiffieHellman:
                    keyPair = GenerateDiffieHellmanKeyPair(random, 256);
                    break;
                case DnsSecAlgorithm.Dsa:
                case DnsSecAlgorithm.DsaNsec3Sha1:
                    keyPair = GenerateDsaKeyPair(random, 1024); // size must be from 512 - 1024 and a multiple of 64
                    break;
                case DnsSecAlgorithm.EcDsaP256Sha256:
                case DnsSecAlgorithm.EcDsaP384Sha384:
                    keyPair = GenerateEcdsaKeyPair(random); // TODO: Pass curve
                    break;
                case DnsSecAlgorithm.EccGost:
                    keyPair = GenerateGost3410KeyPair(random, 512); // 512 or 1024
                    break;
                case DnsSecAlgorithm.Indirect:
                case DnsSecAlgorithm.PrivateDns:
                case DnsSecAlgorithm.PrivateOid:
                    throw new System.NotImplementedException("Indirect | PrivateDns | PrivateOid");
            }


            publicKey = Org.BouncyCastle.X509.SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(keyPair.Public).GetEncoded();
            privateKey = Org.BouncyCastle.Pkcs.PrivateKeyInfoFactory.CreatePrivateKeyInfo(keyPair.Private).GetEncoded();
        }



        public static void TestKeyPair()
        {
            byte[] privateKey;
            byte[] publicKey;

            GenerateKeypair(DnsSecAlgorithm.DiffieHellman, out privateKey, out publicKey);
            GenerateKeypair(DnsSecAlgorithm.Dsa, out privateKey, out publicKey);
            GenerateKeypair(DnsSecAlgorithm.RsaSha512, out privateKey, out publicKey);
            GenerateKeypair(DnsSecAlgorithm.EcDsaP256Sha256, out privateKey, out publicKey);
            GenerateKeypair(DnsSecAlgorithm.EccGost, out privateKey, out publicKey);

            Org.BouncyCastle.Crypto.AsymmetricKeyParameter privKey = Org.BouncyCastle.Security.PrivateKeyFactory.CreateKey(privateKey);
            Org.BouncyCastle.Crypto.AsymmetricKeyParameter pubKey = Org.BouncyCastle.Security.PublicKeyFactory.CreateKey(publicKey);
        }


        internal static void EncodeUShort(byte[] buffer, ref int currentPosition, ushort value)
        {
            if (System.BitConverter.IsLittleEndian)
            {
                buffer[currentPosition++] = (byte)((value >> 8) & 0xff);
                buffer[currentPosition++] = (byte)(value & 0xff);
            }
            else
            {
                buffer[currentPosition++] = (byte)(value & 0xff);
                buffer[currentPosition++] = (byte)((value >> 8) & 0xff);
            }
        }


        internal static void EncodeByteArray(byte[] messageData, ref int currentPosition, byte[] data, int length)
        {
            if ((data != null) && (length > 0))
            {
                System.Buffer.BlockCopy(data, 0, messageData, currentPosition, length);
                currentPosition += length;
            }
        }

        internal static void EncodeByteArray(byte[] messageData, ref int currentPosition, byte[] data)
        {
            if (data != null)
            {
                EncodeByteArray(messageData, ref currentPosition, data, data.Length);
            }
        }


        public class KeyPairRecord
        {
            public Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair KeyPair;
            
            public byte[] PublicKey;
            public byte[] PrivateKey;


            public DnsSecAlgorithm Algorithm;
            public DnsKeyFlags Flags;
            
            public DnsSecDigestType Digest
            {
                get
                {
                    
                    switch (this.Algorithm)
                    {
                        case DnsSecAlgorithm.Dsa:
                        case DnsSecAlgorithm.DsaNsec3Sha1:
                        case DnsSecAlgorithm.RsaSha1:
                        case DnsSecAlgorithm.RsaSha1Nsec3Sha1:
                            return DnsSecDigestType.Sha1;
                        
                        case DnsSecAlgorithm.EcDsaP256Sha256:
                        case DnsSecAlgorithm.RsaSha256:
                            return DnsSecDigestType.Sha256;
                        
                        case DnsSecAlgorithm.EcDsaP384Sha384:
                            return DnsSecDigestType.Sha384;
                        
                        case DnsSecAlgorithm.EccGost:
                            return DnsSecDigestType.EccGost;
                        
                        case DnsSecAlgorithm.Indirect:
                        case DnsSecAlgorithm.PrivateDns:
                        case DnsSecAlgorithm.PrivateOid:
                        case DnsSecAlgorithm.DiffieHellman:
                        case DnsSecAlgorithm.RsaMd5:
                        case DnsSecAlgorithm.RsaSha512:
                            return DnsSecDigestType.EccGost;
                    }
                    
                    return DnsSecDigestType.Sha1;
                }
            }
            
        }



        /// <summary>
		///   Creates a new signing key pair
		/// </summary>
		/// <param name="name">The name of the key or zone</param>
		/// <param name="recordClass">The record class of the DnsKeyRecord</param>
		/// <param name="timeToLive">The TTL in seconds to the DnsKeyRecord</param>
		/// <param name="flags">The Flags of the DnsKeyRecord</param>
		/// <param name="protocol">The protocol version</param>
		/// <param name="algorithm">The key algorithm</param>
		/// <param name="keyStrength">The key strength or 0 for default strength</param>
		/// <returns></returns>
		public static KeyPairRecord CreateSigningKey(DnsSecAlgorithm algorithm, DnsKeyFlags flags, int keyStrength = 0)
        {
            // Found in DnsKeyRecord.CreateSigningKey
            
            KeyPairRecord rec = new KeyPairRecord();
            rec.Flags = flags;
            rec.Algorithm = algorithm;
            
            /*
	        internal override string RecordDataToString()
            {
	            return (ushort) Flags
			            + " " + Protocol
			            + " " + (byte) Algorithm
			            + " " + PublicKey.ToBase64String();
            }


    DnsRecordBase
		    internal abstract string RecordDataToString();

            public override string ToString()
            {
	            string recordData = RecordDataToString();
	            return Name + " " + TimeToLive + " " + RecordClass.ToShortString() + " " + RecordType.ToShortString() + (String.IsNullOrEmpty(recordData) ? "" : " " + recordData);
            }


    RrSigRecord.cs
            byte[] signBuffer;
            int signBufferLength;
            EncodeSigningBuffer(records, out signBuffer, out signBufferLength);

            Signature = key.Sign(signBuffer, signBufferLength);

             */


            Org.BouncyCastle.Security.SecureRandom _secureRandom =
                new Org.BouncyCastle.Security.SecureRandom(new Org.BouncyCastle.Crypto.Prng.CryptoApiRandomGenerator());


            // https://csharp.hotexamples.com/examples/Org.BouncyCastle.Crypto.Generators/DsaKeyPairGenerator/GenerateKeyPair/php-dsakeypairgenerator-generatekeypair-method-examples.html

            
            switch (algorithm)
            {
                case DnsSecAlgorithm.RsaSha1:
                case DnsSecAlgorithm.RsaSha1Nsec3Sha1:
                case DnsSecAlgorithm.RsaSha256:
                case DnsSecAlgorithm.RsaSha512:
                    if (keyStrength == 0)
                        keyStrength = (flags == (DnsKeyFlags.Zone | DnsKeyFlags.SecureEntryPoint)) ? 2048 : 1024;

                    Org.BouncyCastle.Crypto.Generators.RsaKeyPairGenerator rsaKeyGen = new Org.BouncyCastle.Crypto.Generators.RsaKeyPairGenerator();
                    rsaKeyGen.Init(new Org.BouncyCastle.Crypto.KeyGenerationParameters(_secureRandom, keyStrength));
                    var rsaKey = rsaKeyGen.GenerateKeyPair();
                    rec.KeyPair = rsaKey;
                    rec.PrivateKey = Org.BouncyCastle.Pkcs.PrivateKeyInfoFactory.CreatePrivateKeyInfo(rsaKey.Private).GetDerEncoded();
                    var rsaPublicKey = (Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters)rsaKey.Public;
                    var rsaExponent = rsaPublicKey.Exponent.ToByteArrayUnsigned();
                    var rsaModulus = rsaPublicKey.Modulus.ToByteArrayUnsigned();

                    int offset = 1;
                    if (rsaExponent.Length > 255)
                    {
                        rec.PublicKey = new byte[3 + rsaExponent.Length + rsaModulus.Length];
                        EncodeUShort(rec.PublicKey, ref offset, (ushort)rec.PublicKey.Length);
                    }
                    else
                    {
                        rec.PublicKey = new byte[1 + rsaExponent.Length + rsaModulus.Length];
                        rec.PublicKey[0] = (byte)rsaExponent.Length;
                    }
                    EncodeByteArray(rec.PublicKey, ref offset, rsaExponent);
                    EncodeByteArray(rec.PublicKey, ref offset, rsaModulus);
                    break;

                case DnsSecAlgorithm.Dsa:
                case DnsSecAlgorithm.DsaNsec3Sha1:
                    if (keyStrength == 0)
                        keyStrength = 1024;

                    Org.BouncyCastle.Crypto.Generators.DsaParametersGenerator dsaParamsGen = new Org.BouncyCastle.Crypto.Generators.DsaParametersGenerator();
                    dsaParamsGen.Init(keyStrength, 12, _secureRandom);
                    Org.BouncyCastle.Crypto.Generators.DsaKeyPairGenerator dsaKeyGen = new Org.BouncyCastle.Crypto.Generators.DsaKeyPairGenerator();
                    dsaKeyGen.Init(new Org.BouncyCastle.Crypto.Parameters.DsaKeyGenerationParameters(_secureRandom, dsaParamsGen.GenerateParameters()));
                    Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair dsaKey = dsaKeyGen.GenerateKeyPair();
                    rec.KeyPair = dsaKey;
                    
                    rec.PrivateKey = Org.BouncyCastle.Pkcs.PrivateKeyInfoFactory.CreatePrivateKeyInfo(dsaKey.Private).GetDerEncoded();
                    var dsaPublicKey = (Org.BouncyCastle.Crypto.Parameters.DsaPublicKeyParameters)dsaKey.Public;

                    byte[] dsaY = dsaPublicKey.Y.ToByteArrayUnsigned();
                    byte[] dsaP = dsaPublicKey.Parameters.P.ToByteArrayUnsigned();
                    byte[] dsaQ = dsaPublicKey.Parameters.Q.ToByteArrayUnsigned();
                    byte[] dsaG = dsaPublicKey.Parameters.G.ToByteArrayUnsigned();
                    byte dsaT = (byte)((dsaY.Length - 64) / 8);

                    rec.PublicKey = new byte[21 + 3 * dsaY.Length];
                    rec.PublicKey[0] = dsaT;
                    dsaQ.CopyTo(rec.PublicKey, 1);
                    dsaP.CopyTo(rec.PublicKey, 21);
                    dsaG.CopyTo(rec.PublicKey, 21 + dsaY.Length);
                    dsaY.CopyTo(rec.PublicKey, 21 + 2 * dsaY.Length);
                    break;

                case DnsSecAlgorithm.EccGost:
                    Org.BouncyCastle.Crypto.Parameters.ECDomainParameters gostEcDomainParameters = Org.BouncyCastle.Asn1.CryptoPro.ECGost3410NamedCurves.GetByOid(Org.BouncyCastle.Asn1.CryptoPro.CryptoProObjectIdentifiers.GostR3410x2001CryptoProA);

                    var gostKeyGen = new Org.BouncyCastle.Crypto.Generators.ECKeyPairGenerator();
                    gostKeyGen.Init(new Org.BouncyCastle.Crypto.Parameters.ECKeyGenerationParameters(gostEcDomainParameters, _secureRandom));

                    var gostKey = gostKeyGen.GenerateKeyPair();
                    rec.KeyPair = gostKey;
                    rec.PrivateKey = Org.BouncyCastle.Pkcs.PrivateKeyInfoFactory.CreatePrivateKeyInfo(gostKey.Private).GetDerEncoded();
                    var gostPublicKey = (Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters)gostKey.Public;

                    rec.PublicKey = new byte[64];

                    // gostPublicKey.Q.X.ToBigInteger().ToByteArrayUnsigned().CopyTo(publicKey, 32);
                    gostPublicKey.Q.AffineXCoord.ToBigInteger().ToByteArrayUnsigned().CopyTo(rec.PublicKey, 32);
                    // gostPublicKey.Q.Y.ToBigInteger().ToByteArrayUnsigned().CopyTo(publicKey, 0);
                    gostPublicKey.Q.AffineYCoord.ToBigInteger().ToByteArrayUnsigned().CopyTo(rec.PublicKey, 0);

                    System.Array.Reverse(rec.PublicKey);
                    break;
                case DnsSecAlgorithm.EcDsaP256Sha256:
                case DnsSecAlgorithm.EcDsaP384Sha384:
                    int ecDsaDigestSize;
                    Org.BouncyCastle.Asn1.X9.X9ECParameters ecDsaCurveParameter;

                    if (algorithm == DnsSecAlgorithm.EcDsaP256Sha256)
                    {
                        ecDsaDigestSize = new Org.BouncyCastle.Crypto.Digests.Sha256Digest().GetDigestSize();
                        ecDsaCurveParameter = Org.BouncyCastle.Asn1.Nist.NistNamedCurves.GetByOid(Org.BouncyCastle.Asn1.Sec.SecObjectIdentifiers.SecP256r1);
                    }
                    else
                    {
                        ecDsaDigestSize = new Org.BouncyCastle.Crypto.Digests.Sha384Digest().GetDigestSize();
                        ecDsaCurveParameter = Org.BouncyCastle.Asn1.Nist.NistNamedCurves.GetByOid(Org.BouncyCastle.Asn1.Sec.SecObjectIdentifiers.SecP384r1);
                    }

                    Org.BouncyCastle.Crypto.Parameters.ECDomainParameters ecDsaP384EcDomainParameters = new Org.BouncyCastle.Crypto.Parameters.ECDomainParameters(
                        ecDsaCurveParameter.Curve,
                        ecDsaCurveParameter.G,
                        ecDsaCurveParameter.N,
                        ecDsaCurveParameter.H,
                        ecDsaCurveParameter.GetSeed());

                    var ecDsaKeyGen = new Org.BouncyCastle.Crypto.Generators.ECKeyPairGenerator();
                    ecDsaKeyGen.Init(new Org.BouncyCastle.Crypto.Parameters.ECKeyGenerationParameters(ecDsaP384EcDomainParameters, _secureRandom));

                    var ecDsaKey = ecDsaKeyGen.GenerateKeyPair();
                    rec.KeyPair = ecDsaKey;
                    
                    rec.PrivateKey = Org.BouncyCastle.Pkcs.PrivateKeyInfoFactory.CreatePrivateKeyInfo(ecDsaKey.Private).GetDerEncoded();
                    var ecDsaPublicKey = (Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters)ecDsaKey.Public;

                    rec.PublicKey = new byte[ecDsaDigestSize * 2];
                    // ecDsaPublicKey.Q.X.ToBigInteger().ToByteArrayUnsigned().CopyTo(publicKey, 0);
                    ecDsaPublicKey.Q.AffineXCoord.ToBigInteger().ToByteArrayUnsigned().CopyTo(rec.PublicKey, 0);
                    // ecDsaPublicKey.Q.Y.ToBigInteger().ToByteArrayUnsigned().CopyTo(publicKey, ecDsaDigestSize);
                    ecDsaPublicKey.Q.AffineYCoord.ToBigInteger().ToByteArrayUnsigned().CopyTo(rec.PublicKey, ecDsaDigestSize);
                    break;

                default:
                    throw new System.NotSupportedException();
            }
            
            // return new DnsKeyRecord(name, recordClass, timeToLive, flags, protocol, algorithm, rec.PublicKey, rec.PrivateKey);
            return rec;
        }



        public static void PrintAlgorithms()
        {
            
            
            DnsSecAlgorithm[] a = (DnsSecAlgorithm[]) System.Enum.GetValues(typeof(DnsSecAlgorithm));

            for (int i = 0; i < a.Length; ++i)
            {
                string name = a[i].ToString();
                byte value = (byte) a[i];
                
                System.Console.WriteLine(name+": " + value.ToString());
            }

            // foreach (DnsSecAlgorithm[] suit in (DnsSecAlgorithm[]) System.Enum.GetValues(typeof(DnsSecAlgorithm))) { }
            
            
            foreach (string name in System.Enum.GetNames(typeof(DnsSecAlgorithm)))
            {
                System.Console.WriteLine(name);
            }
            
            foreach (string name in System.Enum.GetValues(typeof(DnsSecAlgorithm)))
            {
                System.Console.WriteLine(name);
            }
        }
        
        
        static void Main(string[] args)
        {
            TestKeyPair();

            // https://www.cloudflare.com/dns/dnssec/how-dnssec-works/
            // RRSIG - Contains a cryptographic signature
            // DNSKEY - Contains a public signing key
            // DS - Contains the hash of a DNSKEY record
            // NSEC and NSEC3 - For explicit denial-of-existence of a DNS record
            // CDNSKEY and CDS - For a child zone requesting updates to DS record(s) in the parent zone.


            // The first step towards securing a zone with DNSSEC 
            // is to group all the records (on the same label?) with the same type into a resource record set(RRset). 
            // It’s actually this full RRset that gets digitally signed, opposed to individual DNS records.
            // Of course, this also means that you must request and validate all of the AAAA records 
            // from a zone with the same label instead of validating only one of them.


            // zone-signing key (ZSK)pair:
            // the private portion of the key digitally signs each RRset in the zone, 
            // while the public portion verifies the signature.
            // a zone operator creates digital signatures for each RRset using the private ZSK 
            // and stores them in their name server as RRSIG records.

            // The zone operator also needs to make their public ZSK available by adding it to their name server in a DNSKEY record.

            // the name server also returns the corresponding RRSIG. 
            // The resolver can then pull the DNSKEY record containing the public ZSK from the name server.
            // Together, the RRset, RRSIG, and public ZSK can validate the response.

            // If we trust the zone - signing key in the DNSKEY record, we can trust all the records in the zone. 
            // But, what if the zone - signing key was compromised? We need a way to validate the public ZSK.

            // Key-Signing Keys (KSK): 
            // The KSK validates the DNSKEY record in exactly the same way as our ZSK secured the rest of our RRsets. 
            // It signs the public ZSK (which is stored in a DNSKEY record), creating an RRSIG for the DNSKEY.

            // Just like the public ZSK, the name server publishes the public KSK in another DNSKEY record, 
            // which gives us the DNSKEY RRset shown above. 
            // Both the public KSK and public ZSK are signed by the private KSK. 
            // Resolvers can then use the public KSK to validate the public ZSK.

            // Complicating things further, the key-signing key is signed by itself, which doesn’t provide any additional trust.
            // We need a way to connect the trust in our zone with its parent zone.


            // System.Security.Cryptography.X509Certificates.X509Certificate2 cert2 = new System.Security.Cryptography.X509Certificates.X509Certificate2(byte[] rawData);
            // System.Security.Cryptography.X509Certificates.X509Certificate2 cert2 = DotNetUtilities.CreateX509Cert2("mycert");
            // SecurityKey secKey = new X509SecurityKey(cert2);

            // https://tools.ietf.org/html/rfc4034
            // https://www.dynu.com/Resources/DNS-Records/DNSKEY-Record


            var aaa = new AaaaRecord(DomainName.Parse("example.com"), 0, System.Net.IPAddress.Parse("127.0.0.1"));
            string straaa = aaa.ToString();
            System.Console.WriteLine(straaa);

            
            
            // DnsRecordBase drb = null;

            // DnsMessage msg = DnsMessage.Parse(new byte[] { });
            
            // DnsKeyFlags flags = DnsKeyFlags.SecureEntryPoint;
            KeyPairRecord keyPair = CreateSigningKey( DnsSecAlgorithm.EccGost, DnsKeyFlags.SecureEntryPoint, 512);
            
            
            
            // Private key only necessary when signing, now when publishing... 
            DnsKeyRecord dnsKey = new DnsKeyRecord(
                DomainName.Parse("example.com") // Name: It defines the hostname of a record and whether the hostname will be appended to the label. 
                // Fully qualified hostnames terminated by a period will not append the origin.
                ,RecordClass.Any
                ,60 // ttl The time-to-live in seconds. It specifies how long a resolver is supposed to cache or remember the DNS query 
                // before the query expires and a new one needs to be done.
                ,keyPair.Flags
                ,3 // Fixed value of 3 (for backwards compatibility)
                , keyPair.Algorithm // The public key's cryptographic algorithm.
                , keyPair.PublicKey //  new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 } // Public key data.
                , keyPair.PrivateKey
            );

            
            
            
            string strDNSKey = dnsKey.ToString();
            // dnsKey.CalculateKeyTag()
            System.Console.WriteLine(strDNSKey);
            // example.com. 60 * DNSKEY 256 3 8 AQIDBAUGBwgJ

            
            
            List<DnsRecordBase> records = new List<DnsRecordBase>();
            records.Add(aaa);
            
            
            
            RrSigRecord rrsig1 = RrSigRecord.SignRecord(records, dnsKey, System.DateTime.UtcNow, System.DateTime.UtcNow.AddDays(30));
            string strRRsig = rrsig1.ToString();
            // rrsig1.Signature
            System.Console.WriteLine(strRRsig);
            
            // example.com. 0 IN RRSIG AAAA 12 2 0 20200122193048 20191223193048 46296 example.com. 9aCosjMmgc1iL4jNavgPAA5NXRp5jukyKxb9vCA8PNoz1d4LjaTjfURxnKhX97KkkTdSW0tUoeYgBK7t/qjOFg==
            
            RrSigRecord rrsig = new RrSigRecord(
                    DomainName.Parse("example.com") // Name of the digitally signed RRs
                    , RecordClass.Any
                    , 60 // ttl The time-to-live in seconds. It specifies how long a resolver is supposed to cache or remember the DNS query 
                    // before the query expires and a new one needs to be done.
                    , RecordType.A   // Type Covered: DNS record type that this signature covers.
                    , DnsSecAlgorithm.EccGost // Cryptographic algorithm used to create the signature.
                    , 4 // Labels: Number of labels in the original RRSIG-record name (used to validate wildcards).
                    , 0 // Original TTL: TTL value of the covered record set.
                    , System.DateTime.Now.AddMinutes(1) // Signature Expiration: When the signature expires.
                    , System.DateTime.Now // Signature Inception: When the signature was created.
                    , 0 // Key Tag: A short numeric value which can help quickly identify the DNSKEY-record which can be used to validate this signature.
                    // identifiziert den unterzeichnenden DNSKEY, um zwischen mehreren Signaturen zu unterscheiden (engl. key tag)
                    , DomainName.Parse("example.com") // Signer's Name: Name of the DNSKEY-record which can be used to validate this signature.
                    , new byte[] { 1, 2, 3 } // Signature: Cryptographic signature.  (Base64)
                );


            
            
            
            DsRecord signedDsRec = new DsRecord(dnsKey, 0, keyPair.Digest);
            string strSignedDsRec = signedDsRec.ToString();
            System.Console.WriteLine(strSignedDsRec);
            // signedDsRec.Digest
            // example.com. 0 * DS 24280 12 3 C453FBE75917C8A07BB767230463FA6C271E21D3D92F1ACCC538A194A7C41CC8
            
            
            DsRecord dsRec = new DsRecord(
                  DomainName.Parse("example.com") // Name: It defines the hostname of a record and whether the hostname will be appended to the label. 
                                                 // Fully qualified hostnames terminated by a period will not append the origin.
                , RecordClass.Any
                , 60 // ttl The time-to-live in seconds. It specifies how long a resolver is supposed to cache or remember the DNS query 
                     // before the query expires and a new one needs to be done.
                , 0 // Key Tag: A short numeric value which can help quickly identify the referenced DNSKEY-record.
                , DnsSecAlgorithm.RsaSha256 // The algorithm of the referenced DNSKEY-record.
                , DnsSecDigestType.Sha256 // Digest Type: Cryptographic hash algorithm used to create the Digest value.
                , new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 0xFF } // A cryptographic hash value of the referenced DNSKEY-record.
            );
            
            // dsRec.Digest
            string strDsRec = dsRec.ToString();
            System.Console.WriteLine(strDsRec);
            // example.com. 60 * DS 0 8 2 0102030405060708090AFF
            
            
            string strDS = dsRec.ToString();
            System.Console.WriteLine(strDS);
            // . 0 IN AAAA 127.0.0.1 // aaa
            // example.com. 0 IN AAAA 127.0.0.1
            // ds: 
            // example.com. 60 * DS 0 8 2 010203
            // example.com. 60 * DS 0 8 2 010203040506070809
            // example.com. 60 * DS 0 8 2 0102030405060708090AFF
            
            
            
            
            
            
            // rec.Algorithm

            string key = @"AQPSKmynfzW4kyBv015MUG2DeIQ3
              Cbl+BBZH4b/0PY1kxkmvHjcZc8no
              kfzj31GajIQKY+5CptLr3buXA10h
              WqTkF7H6RfoRqXQeogmMHfpftf6z
              Mv1LyBUgia7za6ZEzOJBOztyvhjL
              742iU/TpPSEDhm2SNKLijfUppn1U
              aNvv4w== ";


            byte[] keyBytes = Base64ToByteArray(key);



            string signature = @"2BB183AF5F22588179A53B0A98631FAD1A292118";


            // ArsoftTestServer.KeyConversion.fromPublicKey()
            PublicKey pk = ArsoftTestServer.KeyConversionTo.toPublicKey(keyBytes, DnsSecAlgorithm.RsaSha1);
            System.Console.WriteLine(pk);


            byte[] generatedKeyBytes = ArsoftTestServer.KeyConversion.fromPublicKey(pk, DnsSecAlgorithm.RsaSha1);


            // ArsoftTestServer.Resolvers.Test4();
            // ArsoftTestServer.SimpleServer.Test();

            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
        }


    }


}
