CREATE TABLE `searches` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `find` varchar(1000) NOT NULL,
  `minSuffixArrayIdx` bigint NOT NULL,
  `maxSuffixArrayIdx` bigint NOT NULL,
  `resultId` int NOT NULL,
  `justCount` tinyint NOT NULL,
  `clientIp` varchar(39) NOT NULL COMMENT '39 for IPv6',
  `searchDate` datetime NOT NULL,
  `processingTimeMs` bigint NOT NULL,
  `numSurroundingDigits` int NULL,
  `namedDigits` varchar(1000) NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=latin1;