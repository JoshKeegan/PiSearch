CREATE TABLE `searches` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `find` varchar(1000) NOT NULL,
  `minSuffixArrayIdx` bigint(20) NOT NULL,
  `maxSuffixArrayIdx` bigint(20) NOT NULL,
  `resultId` int(11) NOT NULL,
  `justCount` tinyint(1) NOT NULL,
  `clientIp` varchar(39) NOT NULL COMMENT '39 for IPv6',
  `searchDate` datetime NOT NULL,
  `processingTimeMs` bigint(20) NOT NULL,
  `numSurroundingDigits` int(11) NOT NULL DEFAULT '20',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=latin1;