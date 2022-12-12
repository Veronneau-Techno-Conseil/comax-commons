namespace Referee.Contracts
{
    public class TableVersion
    {
        /// <summary>
        /// The version part of this TableVersion. Monotonically increasing number.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// The etag of this TableVersion, used for validation of table update operations.
        /// </summary>
        public string VersionEtag { get; set; }

        public Orleans.TableVersion ToOrleans()
        {
            return new Orleans.TableVersion(this.Version, this.VersionEtag);
        }

        public static TableVersion Parse(Orleans.TableVersion tableVersion)
        {
            return new TableVersion
            {
                Version = tableVersion.Version,
                VersionEtag = tableVersion.VersionEtag
            };
        }
    }
}
