
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.T_Domain') AND type in (N'U'))
BEGIN
CREATE TABLE dbo.T_Domain
(
	 DOM_Id bigint NOT NULL
	,DOM_Name character varying(4000) NULL
	,CONSTRAINT PK_T_Domain PRIMARY KEY (DOM_Id)
); 
END


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.T_RecordType') AND type in (N'U'))
BEGIN
CREATE TABLE dbo.T_RecordType
(
	 RT_Id int NOT NULL
	,RT_Name character varying(10) NULL
	,RT_DefiningRFC character varying(4000) NULL
	,RC_Description national character varying(4000) NULL
	,RC_Function national character varying(4000) NULL
	,RC_Source character varying(1000) NULL
	,CONSTRAINT PK_T_RecordType PRIMARY KEY (RT_Id)
); 
END


INSERT INTO T_RecordType (RT_Id, RT_Name, RT_DefiningRFC, RC_Description, RC_Function, RC_Source) VALUES (1, N'A', N'RFC 1035', N'Address record', N'Returns a 32-bit IPv4 address, most commonly used to map hostnames to an IP address of the host, but it is also used for DNSBLs, storing subnet masks in RFC 1101, etc.', N'https://en.wikipedia.org/wiki/List_of_DNS_record_types');
INSERT INTO T_RecordType (RT_Id, RT_Name, RT_DefiningRFC, RC_Description, RC_Function, RC_Source) VALUES (2, N'NS', N'RFC 1035', N'Name server record', N'Delegates a DNS zone to use the given authoritative name servers', N'https://en.wikipedia.org/wiki/List_of_DNS_record_types');
INSERT INTO T_RecordType (RT_Id, RT_Name, RT_DefiningRFC, RC_Description, RC_Function, RC_Source) VALUES (5, N'CNAME', N'RFC 1035', N'Canonical name record', N'Alias of one name to another: the DNS lookup will continue by retrying the lookup with the new name.', N'https://en.wikipedia.org/wiki/List_of_DNS_record_types');
INSERT INTO T_RecordType (RT_Id, RT_Name, RT_DefiningRFC, RC_Description, RC_Function, RC_Source) VALUES (6, N'SOA', N'RFC 1035 and RFC 2308', N'Start of [a zone of] authority record', N'Specifies authoritative information about a DNS zone, including the primary name server, the email of the domain administrator, the domain serial number, and several timers relating to refreshing the zone.', N'https://en.wikipedia.org/wiki/List_of_DNS_record_types');
INSERT INTO T_RecordType (RT_Id, RT_Name, RT_DefiningRFC, RC_Description, RC_Function, RC_Source) VALUES (12, N'PTR', N'RFC 1035', N'Pointer record', N'Pointer to a canonical name. Unlike a CNAME, DNS processing stops and just the name is returned. The most common use is for implementing reverse DNS lookups, but other uses include such things as DNS-SD.', N'https://en.wikipedia.org/wiki/List_of_DNS_record_types');
INSERT INTO T_RecordType (RT_Id, RT_Name, RT_DefiningRFC, RC_Description, RC_Function, RC_Source) VALUES (15, N'MX', N'RFC 1035 and RFC 7505', N'Mail exchange record', N'Maps a domain name to a list of message transfer agents for that domain', N'https://en.wikipedia.org/wiki/List_of_DNS_record_types');
INSERT INTO T_RecordType (RT_Id, RT_Name, RT_DefiningRFC, RC_Description, RC_Function, RC_Source) VALUES (16, N'TXT', N'RFC 1035', N'Text record', N'Originally for arbitrary human-readable text in a DNS record. Since the early 1990s, however, this record more often carries machine-readable data, such as specified by RFC 1464, opportunistic encryption, Sender Policy Framework, DKIM, DMARC, DNS-SD, etc.', N'https://en.wikipedia.org/wiki/List_of_DNS_record_types');
INSERT INTO T_RecordType (RT_Id, RT_Name, RT_DefiningRFC, RC_Description, RC_Function, RC_Source) VALUES (17, N'RP', N'RFC 1183', N'Responsible person', N'Information about the responsible person(s) for the domain. Usually an email address with the @ replaced by a .', N'https://en.wikipedia.org/wiki/List_of_DNS_record_types');
INSERT INTO T_RecordType (RT_Id, RT_Name, RT_DefiningRFC, RC_Description, RC_Function, RC_Source) VALUES (18, N'AFSDB', N'RFC 1183', N'AFS database record', N'Location of database servers of an AFS cell. This record is commonly used by AFS clients to contact AFS cells outside their local domain. A subtype of this record is used by the obsolete DCE/DFS file system.', N'https://en.wikipedia.org/wiki/List_of_DNS_record_types');
INSERT INTO T_RecordType (RT_Id, RT_Name, RT_DefiningRFC, RC_Description, RC_Function, RC_Source) VALUES (24, N'SIG', N'RFC 2535', N'Signature', N'Signature record used in SIG(0) (RFC 2931) and TKEY (RFC 2930).[7] RFC 3755 designated RRSIG as the replacement for SIG for use within DNSSEC.[7]', N'https://en.wikipedia.org/wiki/List_of_DNS_record_types');
INSERT INTO T_RecordType (RT_Id, RT_Name, RT_DefiningRFC, RC_Description, RC_Function, RC_Source) VALUES (25, N'KEY', N'RFC 2535 and RFC 2930', N'Key record', N'Used only for SIG(0) (RFC 2931) and TKEY (RFC 2930).[5] RFC 3445 eliminated their use for application keys and limited their use to DNSSEC.[6] RFC 3755 designates DNSKEY as the replacement within DNSSEC.[7] RFC 4025 designates IPSECKEY as the replacement for use with IPsec.[8]', N'https://en.wikipedia.org/wiki/List_of_DNS_record_types');
INSERT INTO T_RecordType (RT_Id, RT_Name, RT_DefiningRFC, RC_Description, RC_Function, RC_Source) VALUES (28, N'AAAA', N'RFC 3596', N'IPv6 address record', N'Returns a 128-bit IPv6 address, most commonly used to map hostnames to an IP address of the host.', N'https://en.wikipedia.org/wiki/List_of_DNS_record_types');
INSERT INTO T_RecordType (RT_Id, RT_Name, RT_DefiningRFC, RC_Description, RC_Function, RC_Source) VALUES (29, N'LOC', N'RFC 1876', N'Location record', N'Specifies a geographical location associated with a domain name', N'https://en.wikipedia.org/wiki/List_of_DNS_record_types');
INSERT INTO T_RecordType (RT_Id, RT_Name, RT_DefiningRFC, RC_Description, RC_Function, RC_Source) VALUES (33, N'SRV', N'RFC 2782', N'Service locator', N'Generalized service location record, used for newer protocols instead of creating protocol-specific records such as MX.', N'https://en.wikipedia.org/wiki/List_of_DNS_record_types');
INSERT INTO T_RecordType (RT_Id, RT_Name, RT_DefiningRFC, RC_Description, RC_Function, RC_Source) VALUES (35, N'NAPTR', N'RFC 3403', N'Naming Authority Pointer', N'Allows regular-expression-based rewriting of domain names which can then be used as URIs, further domain names to lookups, etc.', N'https://en.wikipedia.org/wiki/List_of_DNS_record_types');
INSERT INTO T_RecordType (RT_Id, RT_Name, RT_DefiningRFC, RC_Description, RC_Function, RC_Source) VALUES (36, N'KX', N'RFC 2230', N'Key eXchanger record', N'Used with some cryptographic systems (not including DNSSEC) to identify a key management agent for the associated domain-name. Note that this has nothing to do with DNS Security. It is Informational status, rather than being on the IETF standards-track. It has always had limited deployment, but is still in use.', N'https://en.wikipedia.org/wiki/List_of_DNS_record_types');
INSERT INTO T_RecordType (RT_Id, RT_Name, RT_DefiningRFC, RC_Description, RC_Function, RC_Source) VALUES (37, N'CERT', N'RFC 4398', N'Certificate record', N'Stores PKIX, SPKI, PGP, etc.', N'https://en.wikipedia.org/wiki/List_of_DNS_record_types');
INSERT INTO T_RecordType (RT_Id, RT_Name, RT_DefiningRFC, RC_Description, RC_Function, RC_Source) VALUES (39, N'DNAME', N'RFC 2672', N'Delegation Name', N'Alias for a name and all its subnames, unlike CNAME, which is an alias for only the exact name. Like a CNAME record, the DNS lookup will continue by retrying the lookup with the new name.', N'https://en.wikipedia.org/wiki/List_of_DNS_record_types');
INSERT INTO T_RecordType (RT_Id, RT_Name, RT_DefiningRFC, RC_Description, RC_Function, RC_Source) VALUES (41, N'OPT', N'RFC 6891', N'Option', N'This is a "pseudo DNS record type" needed to support EDNS', N'https://en.wikipedia.org/wiki/List_of_DNS_record_types');
INSERT INTO T_RecordType (RT_Id, RT_Name, RT_DefiningRFC, RC_Description, RC_Function, RC_Source) VALUES (42, N'APL', N'RFC 3123', N'Address Prefix List', N'Specify lists of address ranges, e.g. in CIDR format, for various address families. Experimental.', N'https://en.wikipedia.org/wiki/List_of_DNS_record_types');
INSERT INTO T_RecordType (RT_Id, RT_Name, RT_DefiningRFC, RC_Description, RC_Function, RC_Source) VALUES (43, N'DS', N'RFC 4034', N'Delegation signer', N'The record used to identify the DNSSEC signing key of a delegated zone', N'https://en.wikipedia.org/wiki/List_of_DNS_record_types');
INSERT INTO T_RecordType (RT_Id, RT_Name, RT_DefiningRFC, RC_Description, RC_Function, RC_Source) VALUES (44, N'SSHFP', N'RFC 4255', N'SSH Public Key Fingerprint', N'Resource record for publishing SSH public host key fingerprints in the DNS System, in order to aid in verifying the authenticity of the host. RFC 6594 defines ECC SSH keys and SHA-256 hashes. See the IANA SSHFP RR parameters registry for details.', N'https://en.wikipedia.org/wiki/List_of_DNS_record_types');
INSERT INTO T_RecordType (RT_Id, RT_Name, RT_DefiningRFC, RC_Description, RC_Function, RC_Source) VALUES (45, N'IPSECKEY', N'RFC 4025', N'IPsec Key', N'Key record that can be used with IPsec', N'https://en.wikipedia.org/wiki/List_of_DNS_record_types');
INSERT INTO T_RecordType (RT_Id, RT_Name, RT_DefiningRFC, RC_Description, RC_Function, RC_Source) VALUES (46, N'RRSIG', N'RFC 4034', N'DNSSEC signature', N'Signature for a DNSSEC-secured record set. Uses the same format as the SIG record.', N'https://en.wikipedia.org/wiki/List_of_DNS_record_types');
INSERT INTO T_RecordType (RT_Id, RT_Name, RT_DefiningRFC, RC_Description, RC_Function, RC_Source) VALUES (47, N'NSEC', N'RFC 4034', N'Next-Secure record', N'Part of DNSSEC—used to prove a name does not exist. Uses the same format as the (obsolete) NXT record.', N'https://en.wikipedia.org/wiki/List_of_DNS_record_types');
INSERT INTO T_RecordType (RT_Id, RT_Name, RT_DefiningRFC, RC_Description, RC_Function, RC_Source) VALUES (48, N'DNSKEY', N'RFC 4034', N'DNS Key record', N'The key record used in DNSSEC. Uses the same format as the KEY record.', N'https://en.wikipedia.org/wiki/List_of_DNS_record_types');
INSERT INTO T_RecordType (RT_Id, RT_Name, RT_DefiningRFC, RC_Description, RC_Function, RC_Source) VALUES (49, N'DHCID', N'RFC 4701', N'DHCP identifier', N'Used in conjunction with the FQDN option to DHCP', N'https://en.wikipedia.org/wiki/List_of_DNS_record_types');
INSERT INTO T_RecordType (RT_Id, RT_Name, RT_DefiningRFC, RC_Description, RC_Function, RC_Source) VALUES (50, N'NSEC3', N'RFC 5155', N'NSEC record version 3', N'An extension to DNSSEC that allows proof of nonexistence for a name without permitting zonewalking', N'https://en.wikipedia.org/wiki/List_of_DNS_record_types');
INSERT INTO T_RecordType (RT_Id, RT_Name, RT_DefiningRFC, RC_Description, RC_Function, RC_Source) VALUES (51, N'NSEC3PARAM', N'RFC 5155', N'NSEC3 parameters', N'Parameter record for use with NSEC3', N'https://en.wikipedia.org/wiki/List_of_DNS_record_types');
INSERT INTO T_RecordType (RT_Id, RT_Name, RT_DefiningRFC, RC_Description, RC_Function, RC_Source) VALUES (52, N'TLSA', N'RFC 6698', N'TLSA certificate association', N'A record for DNS-based Authentication of Named Entities (DANE). RFC 6698 defines "The TLSA DNS resource record is used to associate a TLS server certificate or public key with the domain name where the record is found, thus forming a ''TLSA certificate association''".', N'https://en.wikipedia.org/wiki/List_of_DNS_record_types');
INSERT INTO T_RecordType (RT_Id, RT_Name, RT_DefiningRFC, RC_Description, RC_Function, RC_Source) VALUES (55, N'HIP', N'RFC 5205', N'Host Identity Protocol', N'Method of separating the end-point identifier and locator roles of IP addresses.', N'https://en.wikipedia.org/wiki/List_of_DNS_record_types');
INSERT INTO T_RecordType (RT_Id, RT_Name, RT_DefiningRFC, RC_Description, RC_Function, RC_Source) VALUES (59, N'CDS', N'RFC 7344', N'Child DS', N'Child copy of DS record, for transfer to parent', N'https://en.wikipedia.org/wiki/List_of_DNS_record_types');
INSERT INTO T_RecordType (RT_Id, RT_Name, RT_DefiningRFC, RC_Description, RC_Function, RC_Source) VALUES (60, N'CDNSKEY', N'RFC 7344', N'Child DNSKEY', N'Child copy of DNSKEY record, for transfer to parent', N'https://en.wikipedia.org/wiki/List_of_DNS_record_types');
INSERT INTO T_RecordType (RT_Id, RT_Name, RT_DefiningRFC, RC_Description, RC_Function, RC_Source) VALUES (249, N'TKEY', N'RFC 2930', N'Secret key record', N'A method of providing keying material to be used with TSIG that is encrypted under the public key in an accompanying KEY RR.[10]', N'https://en.wikipedia.org/wiki/List_of_DNS_record_types');
INSERT INTO T_RecordType (RT_Id, RT_Name, RT_DefiningRFC, RC_Description, RC_Function, RC_Source) VALUES (250, N'TSIG', N'RFC 2845', N'Transaction Signature', N'Can be used to authenticate dynamic updates as coming from an approved client, or to authenticate responses as coming from an approved recursive name server[11] similar to DNSSEC.', N'https://en.wikipedia.org/wiki/List_of_DNS_record_types');
INSERT INTO T_RecordType (RT_Id, RT_Name, RT_DefiningRFC, RC_Description, RC_Function, RC_Source) VALUES (251, N'IXFR', N'RFC 1996', N'Incremental Zone Transfer', N'Requests a zone transfer of the given zone but only differences from a previous serial number. This request may be ignored and a full (AXFR) sent in response if the authoritative server is unable to fulfill the request due to configuration or lack of required deltas.', N'https://en.wikipedia.org/wiki/List_of_DNS_record_types');
INSERT INTO T_RecordType (RT_Id, RT_Name, RT_DefiningRFC, RC_Description, RC_Function, RC_Source) VALUES (252, N'AXFR', N'RFC 1035', N'Authoritative Zone Transfer', N'Transfer entire zone file from the master name server to secondary name servers.', N'https://en.wikipedia.org/wiki/List_of_DNS_record_types');
INSERT INTO T_RecordType (RT_Id, RT_Name, RT_DefiningRFC, RC_Description, RC_Function, RC_Source) VALUES (255, N'*', N'RFC 1035', N'All cached records', N'Returns all records of all types known to the name server. If the name server does not have any information on the name, the request will be forwarded on. The records returned may not be complete. For example, if there is both an A and an MX for a name, but the name server has only the A record cached, only the A record will be returned. Sometimes referred to as "ANY", for example in Windows nslookup and Wireshark.', N'https://en.wikipedia.org/wiki/List_of_DNS_record_types');
INSERT INTO T_RecordType (RT_Id, RT_Name, RT_DefiningRFC, RC_Description, RC_Function, RC_Source) VALUES (257, N'CAA', N'RFC 6844', N'Certification Authority Authorization', N'DNS Certification Authority Authorization, constraining acceptable CAs for a host/domain', N'https://en.wikipedia.org/wiki/List_of_DNS_record_types');
INSERT INTO T_RecordType (RT_Id, RT_Name, RT_DefiningRFC, RC_Description, RC_Function, RC_Source) VALUES (32768, N'TA', N'N/A', N'DNSSEC Trust Authorities', N'Part of a deployment proposal for DNSSEC without a signed DNS root. See the IANA database and Weiler Spec for details. Uses the same format as the DS record.', N'https://en.wikipedia.org/wiki/List_of_DNS_record_types');
INSERT INTO T_RecordType (RT_Id, RT_Name, RT_DefiningRFC, RC_Description, RC_Function, RC_Source) VALUES (32769, N'DLV', N'RFC 4431', N'DNSSEC Lookaside Validation record', N'For publishing DNSSEC trust anchors outside of the DNS delegation chain. Uses the same format as the DS record. RFC 5074 describes a way of using these records.', N'https://en.wikipedia.org/wiki/List_of_DNS_record_types');



IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[T_Records]') AND type in (N'U'))
BEGIN
CREATE TABLE dbo.T_Records 
(
	 REC_Id bigint NOT NULL
	,REC_DOM_Id bigint NULL CONSTRAINT FK_T_Records_T_Domain FOREIGN KEY REFERENCES T_Domain(DOM_Id)
	,REC_RT_Id integer NULL CONSTRAINT FK_T_Records_T_RecordType FOREIGN KEY REFERENCES T_RecordType(RT_Id) 
	,REC_Name character varying(4000) NULL
	,REC_Content character varying(1000) NULL
	,REC_ResponsibleName character varying(4000) NULL
	,REC_TTL integer NULL
	,REC_Prio integer NULL
	,REC_Weight integer NULL
	,REC_Port integer NULL
	,REC_SerialNumber integer NULL
	,REC_RefreshInterval integer NULL
	,REC_RetryInterval integer NULL
	,REC_ExpireInterval integer NULL
	,REC_NegativeCachingTTL integer NULL
	,REC_AfsSubType integer NULL
	,REC_ChangeDate bigint NULL
	,CONSTRAINT PK_T_Records PRIMARY KEY (REC_Id)
);
END

/*
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_T_Records_T_Domain]') AND parent_object_id = OBJECT_ID(N'[dbo].[T_Records]'))
ALTER TABLE [dbo].[T_Records]  WITH CHECK ADD  CONSTRAINT [FK_T_Records_T_Domain] FOREIGN KEY([REC_DOM_Id])
REFERENCES [dbo].[T_Domain] ([DOM_Id])
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_T_Records_T_Domain]') AND parent_object_id = OBJECT_ID(N'[dbo].[T_Records]'))
ALTER TABLE [dbo].[T_Records] CHECK CONSTRAINT [FK_T_Records_T_Domain]
GO



IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_T_Records_T_RecordType]') AND parent_object_id = OBJECT_ID(N'[dbo].[T_Records]'))
ALTER TABLE [dbo].[T_Records]  WITH CHECK ADD  CONSTRAINT [FK_T_Records_T_RecordType] FOREIGN KEY([REC_RT_Id])
REFERENCES [dbo].[T_RecordType] ([RT_Id])
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_T_Records_T_RecordType]') AND parent_object_id = OBJECT_ID(N'[dbo].[T_Records]'))
ALTER TABLE [dbo].[T_Records] CHECK CONSTRAINT [FK_T_Records_T_RecordType]
GO
*/







INSERT INTO T_Records(REC_Id, REC_DOM_Id, REC_RT_Id, REC_Name, REC_Content, REC_ResponsibleName, REC_TTL, REC_Prio, REC_Weight, REC_Port, REC_SerialNumber, REC_RefreshInterval, REC_RetryInterval, REC_ExpireInterval, REC_NegativeCachingTTL, REC_AfsSubType, REC_ChangeDate) VALUES (1, NULL, 1, N'vortex.data.microsoft.com.', N'127.0.0.1', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO T_Records(REC_Id, REC_DOM_Id, REC_RT_Id, REC_Name, REC_Content, REC_ResponsibleName, REC_TTL, REC_Prio, REC_Weight, REC_Port, REC_SerialNumber, REC_RefreshInterval, REC_RetryInterval, REC_ExpireInterval, REC_NegativeCachingTTL, REC_AfsSubType, REC_ChangeDate) VALUES (2, NULL, 1, N'license.jetbrains.com.', N'127.0.0.1', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);






IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.T_SshFingerprint') AND type in (N'U'))
BEGIN
CREATE TABLE dbo.T_SshFingerprint 
(
     SSHFP_AlgorithmId integer 
	,SSHFP_AlgorithmName national character varying(4000) 
	,CONSTRAINT PK_T_SshFingerprint PRIMARY KEY (SSHFP_AlgorithmId) 
);
END

INSERT INTO T_SshFingerprint(SSHFP_AlgorithmId, SSHFP_AlgorithmName) VALUES (1, N'RSA');
INSERT INTO T_SshFingerprint(SSHFP_AlgorithmId, SSHFP_AlgorithmName) VALUES (2, N'DSA');
INSERT INTO T_SshFingerprint(SSHFP_AlgorithmId, SSHFP_AlgorithmName) VALUES (3, N'ECDSA');
INSERT INTO T_SshFingerprint(SSHFP_AlgorithmId, SSHFP_AlgorithmName) VALUES (4, N'Ed25519');



IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.T_SshFingerprintHash') AND type in (N'U'))
BEGIN
CREATE TABLE dbo.T_SshFingerprintHash
(
	 SSHFPH_AlgorithmId integer NOT NULL
	,SSHFPH_AlgorithmName national character varying(4000) NULL
	,CONSTRAINT PK_T_SshFingerprintHash PRIMARY KEY (SSHFPH_AlgorithmId) 
);
END
GO


INSERT INTO T_SshFingerprintHash(SSHFPH_AlgorithmId, SSHFPH_AlgorithmName) VALUES (1, N'SHA-1');
INSERT INTO T_SshFingerprintHash(SSHFPH_AlgorithmId, SSHFPH_AlgorithmName) VALUES (2, N'SHA-256');


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.T_AndrewFileSystemSubtype') AND type in (N'U'))
BEGIN
CREATE TABLE dbo.T_AndrewFileSystemSubtype 
(
     AFS_SubtypeId integer 
	,AFS_SubtypeName national character varying(4000) 
	,CONSTRAINT PK_T_AndrewFileSystemSubtype PRIMARY KEY (AFS_SubtypeId) 

);
END 

INSERT INTO T_AndrewFileSystemSubtype(AFS_SubtypeId, AFS_SubtypeName) VALUES (1, N'Afs');
INSERT INTO T_AndrewFileSystemSubtype(AFS_SubtypeId, AFS_SubtypeName) VALUES (2, N'Dce');
