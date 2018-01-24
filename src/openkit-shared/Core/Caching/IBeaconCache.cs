﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Dynatrace.OpenKit.Core.Caching
{
    /// <summary>
    /// Beacon Cache used to cache the Beacons generated by all sessions, actions, ...
    /// </summary>
    public interface IBeaconCache
    {
        /// <summary>
        /// Event raised, for each record added to the BeaconCache.
        /// </summary>
        /// <remarks>
        /// The <code>sender</code> is set to <code>this</code> and the event args is always <code>null</code>.
        /// </remarks>
        event EventHandler RecordAdded;

        /// <summary>
        /// Get a <code>HashSet</code> of currently inserted Beacon IDs.
        /// </summary>
        /// <remarks>
        /// The return value is a snaphsot of currently inserted Beacon IDs.
        /// All changees made after this call are not reflected in the returned structure.
        /// </remarks>
        HashSet<int> BeaconIDs { get; }

        /// <summary>
        /// Get number of bytes currently stored in cache.
        /// </summary>
        long NumBytesInCache { get; }

        /// <summary>
        /// Add event data for a given <paramref name="beaconID"/> to this cache.
        /// </summary>
        /// <param name="beaconID">The beacon's ID (aka Session ID) for which to add event data.</param>
        /// <param name="timestamp">The data's timestamp.</param>
        /// <param name="data">Serialized event data to add.</param>
        void AddEventData(int beaconID, long timestamp, string data);

        /// <summary>
        /// Add action data for a given <paramref name="beaconID"/> to this cache.
        /// </summary>
        /// <param name="beaconID">The beacon's ID (aka Session ID) for which to add event data.</param>
        /// <param name="timestamp">The data's timestamp.</param>
        /// <param name="data">Serialized event data to add.</param>
        void AddActionData(int beaconID, long timestamp, string data);

        /// <summary>
        /// Delete a cache entry for a given <paramref name="beaconID"/>
        /// </summary>
        /// <param name="beaconID">The beacon's ID (aka Session ID) which to delete.</param>
        void DeleteCacheEntry(int beaconID);

        /// <summary>
        /// Get the next chunk for sending to the backend.
        /// </summary>
        /// <remarks>
        /// Note: This method must only be invoked from the beacon sending thread.
        /// </remarks>
        /// <param name="beaconID">The beacon id for which to get the next chunk.</param>
        /// <param name="chunkPrefix">Prefix to append to the beginning of the chunk.</param>
        /// <param name="maxSize">Maximum chunk size. As soon as chunk's size >= maxSize result is returned.</param>
        /// <param name="delimiter">Delimiter between consecutive chunks.</param>
        /// <returns>
        /// <code>null</code> if given <code>beaconID</code> does not exist, an mepty string, if there is no more data to send
        /// or the next chunk to send.
        /// </returns>
        string GetNextBeaconChunk(int beaconID, string chunkPrefix, int maxSize, char delimiter);

        /// <summary>
        /// Remove all data that was previously included in chunks.
        /// </summary>
        /// <remarks>
        /// This method must be called, when data retrieved via <see cref="GetNextBeaconChunk(int, string, int, char)"/> was
        /// successfully sent to the backend, otherwise subsequent calls to <see cref="GetNextBeaconChunk(int, string, int, char)"/>
        /// will retrieve the same data again and again.
        /// 
        /// Note: This method must only be invoked from the beacon sending thread.
        /// </remarks>
        /// <param name="beaconID">The beacon id for which to remove already chunked data.</param>
        void RemoveChunkedData(int beaconID);

        /// <summary>
        /// Reset all data that was previously included in chunks.
        /// </summary>
        /// <remarks>
        /// Note: This method must only be invoked from the beacon sending thread.
        /// </remarks>
        /// <param name="beaconID">The beacon id for which to remove already chunked data.</param>
        void ResetChunkedData(int beaconID);

        /// <summary>
        /// Evict <see cref="BeaconCacheRecord"/> by age for a given beacon.
        /// </summary>
        /// <param name="beaconID">Beacon identifier.</param>
        /// <param name="minTimestamp">The minimum timestamp allowed.</param>
        /// <returns>The number of evicted cache records.</returns>
        int EvictRecordsByAge(int beaconID, long minTimestamp);

        /// <summary>
        /// Evict <see cref="BeaconCacheRecord"/> by number for given beacon.
        /// </summary>
        /// <param name="beaconID">Beacon identifier.</param>
        /// <param name="numRecords"></param>
        /// <returns></returns>
        int EvictRecordsByNumber(int beaconID, int numRecords);

        /// <summary>
        /// Tests if an cached entry for <paramref name="beaconID"/> is empty.
        /// </summary>
        /// <param name="beaconID">The beacon's identifier</param>
        /// <returns><code>true</code> if the cached entry is empty, <code>false</code> otherwise.</returns>
        bool IsEmpty(int beaconID);
    }
}